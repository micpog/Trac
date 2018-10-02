﻿using System;

namespace TrackerApp.Models
{
    public class Position
    {
        public Position(Plugin.Geolocator.Abstractions.Position position)
        {
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            Timestamp = position.Timestamp;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
