using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using TrackerApp.BackgroundProcessing;
using Xamarin.Forms;
using Position = TrackerApp.Models.Position;

namespace TrackerApp.Services
{
    public class TrackerService : ITrackerService
    {
        private const double Tolerance = 0.00001;

        private readonly List<Position> _positions = new List<Position>();
        private readonly IPermissionValidator _permissionValidator;
        private readonly IGeolocator _geolocator = CrossGeolocator.Geolocator;

        public TrackerService(IPermissionValidator permissionValidator)
        {
            _permissionValidator = permissionValidator;
            _geolocator.DesiredAccuracy = 1;
        }

        public async Task StartTracking()
        {
            if (!await _permissionValidator.ValidateGeolocationPermission() || _geolocator.IsListening)
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

        public async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentPositon()
        {
            return await _geolocator.GetPositionAsync(TimeSpan.FromMilliseconds(500));
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
    }
}
