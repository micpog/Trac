using Position = TrackerApp.Models.Position;

namespace TrackerApp.BackgroundProcessing
{
    public class NewPositionMessage
    {
        public Position Position { get; set; }
    }
}
