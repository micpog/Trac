using System.Collections.Generic;
using TrackerApp.BackgroundProcessing;
using TrackerApp.Models;
using Xamarin.Forms;

namespace TrackerApp.Services
{
    public class MessagingHandler : IMessagingHandler
    {
        public void SendMessage(List<Position> positions)
        {
            var newPathMessage = new NewPathMessage {Positions = positions};
            Device.BeginInvokeOnMainThread(() => { MessagingCenter.Send<NewPathMessage>(newPathMessage, "NewPathMessage"); });
        }
    }
}