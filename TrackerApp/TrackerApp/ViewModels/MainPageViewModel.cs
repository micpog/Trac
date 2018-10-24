using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TrackerApp.BackgroundProcessing;
using TrackerApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;
using MessagingCenter = Xamarin.Forms.MessagingCenter;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private ObservableCollection<Position> _positions;

        public MainPageViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            StartTrackingCommand = new RelayCommand(StartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking);

            HandleReceivedMessages();
        }

        public RelayCommand StartTrackingCommand { get; }
        public RelayCommand StopTrackingCommand { get; }

        public MoveCameraRequest MoveCameraRequest { get; } = new MoveCameraRequest();

        public ObservableCollection<Polyline> Polylines { get; set; }

        public ObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (value != null)
                {
                    _positions = value;
                    OnPropertyChanged(nameof(Positions));
                }
            }
        }

        private async void StartTracking()
        {
            if (_positions != null && _positions.Any())
            {
                var confirmed = await _dialogService.DisplayAlert("Warning", "Confirm to clear previous path", "Confirm", "Cancel");
                if (!confirmed)
                {
                    return;
                }

                _positions.Clear();
            }

            var message = new StartTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StartTrackingTaskMessage));
        }

        private async void StopTracking()
        {
            var confirmed = await _dialogService.DisplayAlert("Tracking warning", "Do you want to stop tracking?", "Yes", "No");
            if (!confirmed)
            {
                return;
            }

            var message = new StopTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StopTrackingTaskMessage));
        }

        private void DrawPolyline(List<Position> positions)
        {
            var polyline = new Polyline
            {
                StrokeColor = Color.ForestGreen,
                StrokeWidth = 3f
            };

            foreach (var position in positions)
            {
                polyline.Positions.Add(new Xamarin.Forms.GoogleMaps.Position(position.Latitude, position.Longitude));
            }

            Polylines.Add(polyline);
            MoveCameraRequest.MoveCamera
            (CameraUpdateFactory.NewCameraPosition(
                new CameraPosition(
                    new Xamarin.Forms.GoogleMaps.Position(
                        polyline.Positions.First().Latitude, polyline.Positions.First().Longitude), 9d)));
        }

        private void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<NewPathMessage>(this, nameof(NewPathMessage), message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Positions = new ObservableCollection<Position>(message.Positions);
                    DrawPolyline(Positions.ToList());
                });
            });
        }
    }
}
