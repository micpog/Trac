using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using TrackerApp.BackgroundProcessing;
using TrackerApp.Droid.Services;
using MessagingCenter = Xamarin.Forms.MessagingCenter;

namespace TrackerApp.Droid
{
    [Activity(Label = "TrackerApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            CrossCurrentActivity.Current.Activity = this;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);

            LoadApplication(new TrackerApp.App());

            WireUpLongRunningTracking();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void WireUpLongRunningTracking()
        {
            MessagingCenter.Subscribe<StartTrackingTaskMessage>(this, nameof(StartTrackingTaskMessage), message =>
            {
                var intent = new Intent(this, typeof(TrackingService));
                StartForegroundService(intent);
            });

            MessagingCenter.Subscribe<StopTrackingTaskMessage>(this, nameof(StopTrackingTaskMessage), message =>
            {
                var intent = new Intent(this, typeof(TrackingService));
                StopService(intent);
            });
        }
    }
}

