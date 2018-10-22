using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Autofac.Extras.CommonServiceLocator;
using Autofac;
using CommonServiceLocator;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace TrackerApp
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
            var containerBuilder = new ContainerBuilder();;
		    containerBuilder.RegisterModule<TrackerModule>();
		    var container = containerBuilder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
