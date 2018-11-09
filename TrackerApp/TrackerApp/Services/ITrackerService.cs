using System.Collections.Generic;
using System.Threading.Tasks;
using TrackerApp.Models;

namespace TrackerApp.Services
{
    public interface ITrackerService
    {
        Task StartTracking();
        Task StopTracking();
        Task<Plugin.Geolocator.Abstractions.Position> GetCurrentPositon();
        List<Position> Positions { get; }
    }
}