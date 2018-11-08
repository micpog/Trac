using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;
using Position = TrackerApp.Models.Position;

namespace TrackerApp.Services
{
    public class TrackerService : ITrackerService
    {
        private const double Tolerance = 0.00001;

        public List<Position> Positions { get; } = new List<Position>();
        private readonly IPermissionValidator _permissionValidator;
        private readonly IGeolocator _geolocator = CrossGeolocator.Geolocator;
        private readonly IMessagingHandler _messagingHandler;

        public TrackerService(IPermissionValidator permissionValidator, IMessagingHandler messagingHandler)
        {
            _permissionValidator = permissionValidator;
            _messagingHandler = messagingHandler;
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

            if (!Positions.Any() || Positions.Count < 2)
            {
                return;
            }

            _messagingHandler.SendMessage(Positions);
        }

        public async Task<Plugin.Geolocator.Abstractions.Position> GetCurrentPositon()
        {
            return await _geolocator.GetPositionAsync(TimeSpan.FromMilliseconds(500));
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
    }
}
