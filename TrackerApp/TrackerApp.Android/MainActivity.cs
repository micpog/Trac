using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using TrackerApp.BackgroundProcessing;
using MessagingCenter = Xamarin.Forms.MessagingCenter;

namespace TrackerApp.Droid
{
    [Activity(Label = "TrackerApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static readonly string TAG = typeof(MainActivity).FullName;

        private Intent startServiceIntent;
        private Intent stopServiceIntent;
        private bool isStarted;

        protected override void OnCreate(Bundle bundle)
        {
            OnNewIntent(this.Intent);
            if (bundle != null)
            {
                isStarted = bundle.GetBoolean(Consts.SERVICE_STARTED_KEY, false);
            }

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            Xamarin.FormsGoogleMapsBindings.Init();
            CrossCurrentActivity.Current.Init(this, bundle);

            LoadApplication(new TrackerApp.App());

            CreateNotificationChannel();
            WireUpLongRunningTracking();
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            var bundle = intent.Extras;
            if (bundle != null)
            {
                if (bundle.ContainsKey(Consts.SERVICE_STARTED_KEY))
                {
                    isStarted = true;
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(Consts.SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }

        private void WireUpLongRunningTracking()
        {
            var serviceIntent = new Intent(this, typeof(Services.TrackingService));

            MessagingCenter.Subscribe<StartTrackingTaskMessage>(this, nameof(StartTrackingTaskMessage), message =>
            {
                serviceIntent.SetAction(Consts.ACTION_START_SERVICE);
                StartService(serviceIntent);
            });

            MessagingCenter.Subscribe<StopTrackingTaskMessage>(this, nameof(StopTrackingTaskMessage), message =>
            {
                serviceIntent.SetAction(Consts.ACTION_STOP_SERVICE);
                StopService(serviceIntent);
            });
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var serviceChannel = new NotificationChannel(Consts.ChannelID, "Tracking Service Channel", NotificationImportance.Default);

                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(serviceChannel);
            }
        }

    }
}

