using Autofac;
using TrackerApp.Services;

namespace TrackerApp
{
    public class TrackerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TrackerService>().As<ITrackerService>();
            builder.RegisterType<TrackerService>().PropertiesAutowired();
            builder.RegisterType<DialogService>().As<IDialogService>();
        }
    }
}
