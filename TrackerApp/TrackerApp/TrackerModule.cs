using Autofac;
using TrackerApp.Services;

namespace TrackerApp
{
    public class TrackerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TrackerService>().As<ITrackerService>().SingleInstance();
            builder.RegisterType<TrackerService>().PropertiesAutowired().SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<PermissionValidator>().As<IPermissionValidator>();
            builder.RegisterType<MessagingHandler>().As<IMessagingHandler>();
        }
    }
}
