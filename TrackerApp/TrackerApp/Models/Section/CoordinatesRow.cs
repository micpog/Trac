using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.Models.Section
{
    public class CoordinatesRow : ISectionRow
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
