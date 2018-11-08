using System.Collections.Generic;
using TrackerApp.Models;

namespace TrackerApp.Services
{
    public interface IMessagingHandler
    {
        void SendMessage(List<Position> positions);
    }
}