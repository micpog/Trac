using System.Threading.Tasks;

namespace TrackerApp.Services
{
    public interface ITrackerService
    {
        Task StartTracking();
        Task StopTracking();
        Task<Plugin.Geolocator.Abstractions.Position> GetCurrentPositon();
    }
}