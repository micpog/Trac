using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;

namespace TrackerApp
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<BaseViewModel>().AsSelf();
            containerBuilder.RegisterType<MainPageViewModel>().AsSelf();
            containerBuilder.RegisterModule<TrackerModule>();

            var container = containerBuilder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }
    }
}