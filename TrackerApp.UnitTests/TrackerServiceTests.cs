using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Plugin.Geolocator.Abstractions;
using TrackerApp.Services;

namespace TrackerApp.UnitTests
{
    [TestFixture]
    public class TrackerServiceTests
    {
        private TrackerService _trackerService;
        private IPermissionValidator _permissionValidator;
        private IGeolocator _geolocator;

        [SetUp]
        public void Setup()
        {
            _geolocator = CrossGeolocator.Geolocator = Substitute.For<IGeolocator>();
            _permissionValidator = Substitute.For<IPermissionValidator>();
            _trackerService = new TrackerService(_permissionValidator);
        }

        [Test]
        public async Task StartTracking_Should_start_listening_for_changes()
        {
            _permissionValidator.ValidateGeolocationPermission().Returns(true);
            _geolocator.IsListening.Returns(false);

            await _trackerService.StartTracking();

            await _geolocator.Received(1).StartListeningAsync(Arg.Any<TimeSpan>(), Arg.Any<double>());
            _geolocator.Received(1).PositionChanged += Arg.Any<EventHandler<PositionEventArgs>>();
        }

        [Test]
        public async Task StartTracking_Should_return_from_execution_When_geolocation_permission_is_not_validated()
        {
            _permissionValidator.ValidateGeolocationPermission().Returns(false);
            _geolocator.IsListening.Returns(false);

            await _trackerService.StartTracking();

            await _geolocator.DidNotReceive().StartListeningAsync(Arg.Any<TimeSpan>(), Arg.Any<double>());
            _geolocator.DidNotReceive().PositionChanged += Arg.Any<EventHandler<PositionEventArgs>>();
        }

        [Test]
        public async Task StartTracking_Should_return_from_execution_When_geolocator_is_already_listening()
        {
            _permissionValidator.ValidateGeolocationPermission().Returns(true);
            _geolocator.IsListening.Returns(true);

            await _trackerService.StartTracking();

            await _geolocator.DidNotReceive().StartListeningAsync(Arg.Any<TimeSpan>(), Arg.Any<double>());
            _geolocator.DidNotReceive().PositionChanged += Arg.Any<EventHandler<PositionEventArgs>>();
        }

        [Test]
        public async Task StopTracking_Should_return_from_execution_When_geolocator_is_not_already_listening()
        {
            _geolocator.IsListening.Returns(false);

            await _trackerService.StopTracking();

            await _geolocator.DidNotReceive().StopListeningAsync();
            _geolocator.DidNotReceive().PositionChanged -= Arg.Any<EventHandler<PositionEventArgs>>();
        }

        [Test]
        public async Task StopTracking_Should_stop_listening_When_geolocator_is_listening()
        {
            _geolocator.IsListening.Returns(true);

            await _trackerService.StopTracking();

            await _geolocator.Received(1).StopListeningAsync();
            _geolocator.Received(1).PositionChanged -= Arg.Any<EventHandler<PositionEventArgs>>();
        }

        [Test]
        public async Task StopTracking_Should_return_from_execution_When_no_positions_received()
        {
            _geolocator.IsListening.Returns(true);

            await _trackerService.StopTracking();

            
        }
    }
}
