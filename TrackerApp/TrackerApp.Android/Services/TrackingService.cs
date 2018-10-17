using Android.App;
using Android.Content;
using Android.OS;

namespace TrackerApp.Droid.Services
{
    [Service]
    public class TrackingService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);

            var notification = new Notification.Builder(this, Consts.ChannelID)
                .SetContentTitle("RideTracker")
                .SetContentText("I'm tracking you!")
                .SetContentIntent(pendingIntent)
                .Build();

            StartForeground(1, notification);

            return StartCommandResult.NotSticky;
        }
    }
}