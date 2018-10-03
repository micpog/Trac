using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using TrackerApp.Droid.Annotations;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Position> _positions;
        private readonly IGeolocator _geolocator;

        public MainPageViewModel()
        {
            _geolocator = CrossGeolocator.Current;
            StartTrackingCommand = new RelayCommand(StartTracking);
        }

        public RelayCommand StartTrackingCommand { get; set; }

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

            this.Positions = new ObservableCollection<Position>();
            await _geolocator.StartListeningAsync(TimeSpan.FromSeconds(10), 10);
            _geolocator.PositionChanged += PositionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PositionChanged(object sender, PositionEventArgs args)
        {
            Positions.Add(new Position(args.Position));
        }
    }
}
