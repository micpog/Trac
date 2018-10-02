using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Position> Positions;
        private readonly IGeolocator _geolocator;

        public MainPage()
        {
            Positions = new ObservableCollection<Position>();
            _geolocator = CrossGeolocator.Current;
            InitializeComponent();
        }

        private async void Button_StartTracking(object sender, EventArgs e)
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

        private void PositionChanged(object sender, PositionEventArgs args)
        {
            Positions.Add(new Position(args.Position));
        }

        private async void Button_StopTracking(object sender, EventArgs e)
        {
            if (!_geolocator.IsListening)
            {
                return;
            }

            await _geolocator.StopListeningAsync();
            _geolocator.PositionChanged -= PositionChanged;
            this.BindingContext = new MainPageViewModel(Positions);
        }
    }
}
