using Position = TrackerApp.Models.Position;

namespace TrackerApp.Services
{
    public interface ITracker
    {
        Task StartTracking();
        Task StopTracking();
    }

    public class Tracker : ITracker
    {
        private readonly IDialogService _dialogService;
        private const double Tolerance = 0.00001;
        private readonly List<Position> _positions = new List<Position>();
        private readonly IGeolocator _geolocator;

        public Tracker(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _geolocator = CrossGeolocator.Current;
        }

        public async Task StartTracking()
        {
            if (await ValidateGeolocationPermission())
            {
                return;
            }

            if (_geolocator.IsListening)
            {
                return;
            }

            await _geolocator.StartListeningAsync(TimeSpan.FromSeconds(2), 0);
            _geolocator.PositionChanged += PositionChanged;
        }

        public async Task StopTracking()
        {
            if (!_geolocator.IsListening)
            {
                return;
            }

            await _geolocator.StopListeningAsync();
            _geolocator.PositionChanged -= PositionChanged;

            if (!_positions.Any() || _positions.Count < 2)
            {
                return;
            }

            var newPathMessage = new NewPathMessage { Positions = _positions };
            Device.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send<NewPathMessage>(newPathMessage, "NewPathMessage");
            });
        }

        private void PositionChanged(object sender, PositionEventArgs args)
        {
            if (_positions.Any(p => Math.Abs(p.Latitude - args.Position.Latitude) < Tolerance)
                && _positions.Any(p => Math.Abs(p.Longitude - args.Position.Longitude) < Tolerance))
            {
                return;
            }

            _positions.Add(new Position(args.Position));
        }

        private async Task<bool> ValidateGeolocationPermission()
        {
            var current =
                await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            Console.WriteLine("####### Checking location is enabled.");
            if (!_geolocator.IsGeolocationEnabled)
            {
                await _dialogService.DisplayAlert("Location", "Please turn on location services.", "Ok");
                return true;
            }

            var result =
                await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new[]
                    {Plugin.Permissions.Abstractions.Permission.Location});
            var status = result[Plugin.Permissions.Abstractions.Permission.Location];

            Console.WriteLine("####### Checking permissions.");
            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                await _dialogService.DisplayAlert("Location", "Location is required for GPS. Device data cannot be saved.", "Cancel");
                return true;
            }

            return false;
        }

    }
}
