using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TrackerApp.Droid
{
    public class App : Application
    {
        public override void OnCreate()
        {
            base.OnCreate();

            CreateNotificationChannel();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
            {
                NotificationChannel serviceChannel = new NotificationChannel(Consts.ChannelID, "Tracking Service Channel", NotificationImportance.Default);

                NotificationManager manager = (NotificationManager) GetSystemService(NotificationService);
                manager.CreateNotificationChannel(serviceChannel);
            }
        }
    }
}