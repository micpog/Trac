using System;
using System.Collections.ObjectModel;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<Position> _positions;
        private readonly IGeolocator _geolocator;
        private bool _canDisplay;

        public MainPageViewModel()
        {
            _geolocator = CrossGeolocator.Current;
            StartTrackingCommand = new RelayCommand(StartTracking);
            StopTrackingCommand = new RelayCommand(StopTracking);
        }

        public RelayCommand StartTrackingCommand { get; }
        public RelayCommand StopTrackingCommand { get; }

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
            await _geolocator.StartListeningAsync(TimeSpan.FromSeconds(10), 10);
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
        }

        private void PositionChanged(object sender, PositionEventArgs args)
        {
            Positions.Add(new Position(args.Position));
        }
    }
}
