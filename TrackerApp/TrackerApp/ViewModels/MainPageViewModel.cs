using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using TrackerApp.BackgroundProcessing;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;
using MessagingCenter = Xamarin.Forms.MessagingCenter;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<Position> _positions;

        public MainPageViewModel()
        {
            StartTrackingCommand = new RelayCommand(StartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking);

            HandleReceivedMessages();
        }

        public RelayCommand StartTrackingCommand { get; }
        public RelayCommand StopTrackingCommand { get; }

        public MoveCameraRequest MoveCameraRequest { get; } = new MoveCameraRequest();

        public ObservableCollection<Polyline> Polylines { get; set; } = new ObservableCollection<Polyline>();

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

        private void StartTracking()
        {
            var message = new StartTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StartTrackingTaskMessage));
        }

        private void StopTracking()
        {
            var message = new StopTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StopTrackingTaskMessage));
        }

        private void DrawPolyline(List<Position> positions)
        {
            var polyline = new Polyline
            {
                StrokeColor = Color.ForestGreen,
                StrokeWidth = 10f
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
                Device.BeginInvokeOnMainThread(async () =>
                {

                });
            });

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
