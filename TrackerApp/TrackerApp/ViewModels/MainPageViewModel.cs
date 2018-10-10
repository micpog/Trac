using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : BaseViewModel
    {
        private const double Tolerance = 0.00001;

        private ObservableCollection<Position> _positions;
        private IGeolocator _geolocator;
        private Map _map = new Map();
        private bool _canDisplay;

        public MainPageViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            _geolocator = CrossGeolocator.Current;
            StartTrackingCommand = new RelayCommand(StartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking);
        }

        public RelayCommand StartTrackingCommand { get; set; }
        public RelayCommand StopTrackingCommand { get; set; }

        public bool CanDisplay
        {
            get => _canDisplay;
            private set
            {
                _canDisplay = value;
                OnPropertyChanged(nameof(CanDisplay));
            }
        }

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
            Console.WriteLine("####### Checking location is enabled.");
            if (!_geolocator.IsGeolocationEnabled)
            {
                await DisplayAlert("Location", "Please turn on location services.", "Ok");
                return;
            }

            var result = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[] { Plugin.Permissions.Abstractions.Permission.Location });
            var status = result[Plugin.Permissions.Abstractions.Permission.Location];

            Console.WriteLine("####### Checking permissions.");
            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await DisplayAlert("Location", "Location is required for GPS. Device data cannot be saved.", "Cancel");
                return;
            }

            if (_geolocator.IsListening)
            {
                return;
            }

            CanDisplay = false;
            this.Positions = new ObservableCollection<Position>();
            await _geolocator.StartListeningAsync(TimeSpan.FromSeconds(2), 0);
            _geolocator.PositionChanged += PositionChanged;
        }

        private async void StopTracking()
        {
            if (!_geolocator.IsListening)
            {
                return;
            }

            await _geolocator.StopListeningAsync();
            _geolocator.PositionChanged -= PositionChanged;

            CanDisplay = true;
            if (Positions.Count > 1)
            {
                DrawPolyline(Positions.ToList());
            }

            DrawPoint(Positions.ToList());
        }

        private void ResetMap()
        {
            Initialize();
        }

        private void DrawPoint(List<Position> positions)
        {
            var position = positions.First();
            var circle = new Circle
            {
                StrokeColor = Color.DarkGreen,
                StrokeWidth = (float) 0.1d,
                Radius = Distance.FromMeters(500),
                Center = new Xamarin.Forms.GoogleMaps.Position(position.Latitude, position.Longitude)
            };
            _map.Circles.Add(circle);
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

            _map.Polylines.Add(polyline);
        }
    }
}
