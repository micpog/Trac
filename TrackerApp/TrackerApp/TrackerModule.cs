using Autofac;
using TrackerApp.Droid;
using TrackerApp.Services;

namespace TrackerApp
{
    public class TrackerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TrackerService>().As<ITrackerService>();
            builder.RegisterType<DialogService>().As<IDialogService>();
        }
    }
}
