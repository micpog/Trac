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
        private const double Tolerance = 0.00001;

        private ObservableCollection<Position> _positions;
        private readonly IGeolocator _geolocator;

        public MainPageViewModel()
        {
            _geolocator = CrossGeolocator.Current;
            StartTrackingCommand = new RelayCommand(StartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking);

            HandleReceivedMessages();
        }

        public RelayCommand StartTrackingCommand { get; }
        public RelayCommand StopTrackingCommand { get; }

        public MoveCameraRequest MoveCameraRequest { get; set; } = new MoveCameraRequest();

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

        private async void StartTracking()
        {
            if (await ValidateGeolocationPermission())
            {
                return;
            }

            if (_geolocator.IsListening)
            {
                return;
            }

            var message = new StartTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StartTrackingTaskMessage));
        }

        private void StopTracking()
        {
            if (!_geolocator.IsListening)
            {
                return;
            }

            var message = new StopTrackingTaskMessage();
            MessagingCenter.Send(message, nameof(StopTrackingTaskMessage));
        }

        private async Task<bool> ValidateGeolocationPermission()
        {
            Console.WriteLine("####### Checking location is enabled.");
            if (!_geolocator.IsGeolocationEnabled)
            {
                await DisplayAlert("Location", "Please turn on location services.", "Ok");
                return true;
            }

            var result =
                await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[]
                    {Plugin.Permissions.Abstractions.Permission.Location});
            var status = result[Plugin.Permissions.Abstractions.Permission.Location];

            Console.WriteLine("####### Checking permissions.");
            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await DisplayAlert("Location", "Location is required for GPS. Device data cannot be saved.", "Cancel");
                return true;
            }

            return false;
        }

        private void PositionChanged(object sender, PositionEventArgs args)
        {
            if (Positions.Any(p => Math.Abs(p.Latitude - args.Position.Latitude) < Tolerance)
            && Positions.Any(p => Math.Abs(p.Longitude - args.Position.Longitude) < Tolerance))
            {
                return;
            }

            Positions.Add(new Position(args.Position));
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
                    new Xamarin.Forms.GoogleMaps.Position(polyline.Positions.First().Latitude, polyline.Positions.First().Longitude), 9d)));
        }

        private void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<NewPositionMessage>(this, nameof(NewPositionMessage), message =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Positions = new ObservableCollection<Position>();
                    await _geolocator.StartListeningAsync(TimeSpan.FromSeconds(2), 0);
                    _geolocator.PositionChanged += PositionChanged;
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, nameof(CancelledMessage), message =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await _geolocator.StopListeningAsync();
                    _geolocator.PositionChanged -= PositionChanged;

                    if (Positions.Count < 2)
                    {
                        return;
                    }

                    DrawPolyline(Positions.ToList());
                });
            });
        }
    }
}
