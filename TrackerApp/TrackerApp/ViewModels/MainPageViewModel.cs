﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public class MainPageViewModel : BaseViewModel
    {
        private const double Tolerance = 0.00001;

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

        public MoveCameraRequest MoveCameraRequest { get; set; } = new MoveCameraRequest();

        public ObservableCollection<Circle> Circles { get; set; } = new ObservableCollection<Circle>();

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
            else if (Positions.Count == 1)
            {
                DrawPoint(Positions.ToList());

            }
        }

        private void DrawPoint(List<Position> positions)
        {
            var position = positions.First();
            var circle = new Circle
            {
                StrokeColor = Color.DarkGreen,
                StrokeWidth = (float)0.1d,
                Radius = Distance.FromMeters(500),
                Center = new Xamarin.Forms.GoogleMaps.Position(position.Latitude, position.Longitude)
            };

            Circles.Add(circle);
            MoveCameraRequest.MoveCamera
                (CameraUpdateFactory.NewCameraPosition(
                new CameraPosition(
                    new Xamarin.Forms.GoogleMaps.Position(circle.Center.Latitude, circle.Center.Longitude), 17d)));
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
                    new Xamarin.Forms.GoogleMaps.Position(polyline.Positions.First().Latitude, polyline.Positions.First().Longitude), 17d)));
        }
    }
}