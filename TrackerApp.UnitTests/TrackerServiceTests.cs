using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using Plugin.Geolocator.Abstractions;
using TrackerApp.Models;
using TrackerApp.Services;
using Position = Plugin.Geolocator.Abstractions.Position;

namespace TrackerApp.UnitTests
{
    [TestFixture]
    public class TrackerServiceTests
    {
        private TrackerService _trackerService;
        private IPermissionValidator _permissionValidator;
        private IGeolocator _geolocator;
        private IMessagingHandler _messagingHandler;

        [SetUp]
        public void Setup()
        {
            _geolocator = CrossGeolocator.Geolocator = Substitute.For<IGeolocator>();
            _permissionValidator = Substitute.For<IPermissionValidator>();
            _messagingHandler = Substitute.For<IMessagingHandler>();
            _trackerService = new TrackerService(_permissionValidator, _messagingHandler);
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
            var positions = new List<Models.Position>();

            var positionsProperty = typeof(TrackerService).GetField("<Positions>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (positionsProperty != null) positionsProperty.SetValue(_trackerService, positions);

            _geolocator.IsListening.Returns(true);

            await _trackerService.StopTracking();

            _messagingHandler.DidNotReceive().SendMessage(positions);
        }

        [Test]
        public async Task StopTracking_Should_return_from_execution_When_only_one_position_received()
        {
            var positions = new List<Models.Position>
            {
                new Models.Position(new Position(10,10))
            };

            var positionsProperty = typeof(TrackerService).GetField("<Positions>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (positionsProperty != null) positionsProperty.SetValue(_trackerService, positions);

            _geolocator.IsListening.Returns(true);

            await _trackerService.StopTracking();

            _messagingHandler.DidNotReceive().SendMessage(positions);
        }

        [Test]
        public async Task StopTracking_Should_call_SendMessage_When_more_than_two_positions_received()
        {
            var positions = new List<Models.Position>
            {
                new Models.Position(new Position(10, 10)),
                new Models.Position(new Position(20, 20))
            };

            var positionsProperty = typeof(TrackerService).GetField("<Positions>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (positionsProperty != null) positionsProperty.SetValue(_trackerService, positions);

            _geolocator.IsListening.Returns(true);

            await _trackerService.StopTracking();

            _messagingHandler.Received(1).SendMessage(positions);
        }
    }
}
