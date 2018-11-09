using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using CommonServiceLocator;
using TrackerApp.Services;
namespace TrackerApp.Droid.Services
{
    [Service]
    public class TrackingService : Service
    {
        public ITrackerService TrackerService => ServiceLocator.Current.GetInstance<TrackerService>();

        public static string TAG = typeof(TrackingService).FullName;

        private Handler handler;
        private bool isStarted;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            var currentPositon = TrackerService.GetCurrentPositon();
            
        }

        public override void OnDestroy()
        {
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");
            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.Cancel(Consts.SERVICE_RUNNING_NOTIFICATION_ID);
            TrackerService.StopTracking();
            isStarted = false;
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action.Contains(Consts.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: Service is already running.");
                }
                else
                {
                    Log.Info(TAG, "OnStartCommand: Service is starting.");
                    RegisterForegroundService();
                    TrackerService.StartTracking();
                }
            }
            else if (intent.Action.Equals(Consts.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, "OnStartCommand: Service is stopping.");
                StopForeground(true);
                StopSelf();
                isStarted = false;
            }

            return StartCommandResult.NotSticky;
        }

        private void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this, Consts.ChannelID)
                .SetContentTitle("RideTracker")
                .SetContentText("I'm tracking you!")
                .SetSmallIcon(Resource.Drawable.location)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .Build();

            StartForeground(Consts.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        private PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Consts.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(Consts.SERVICE_STARTED_KEY, true);

            return PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
        }
    }
}
