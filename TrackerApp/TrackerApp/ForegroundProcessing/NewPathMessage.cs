using System.Collections.Generic;
using Position = TrackerApp.Models.Position;

namespace TrackerApp.BackgroundProcessing
{
    public class NewPathMessage
    {
        public List<Position> Positions { get; set; }
    }
}
