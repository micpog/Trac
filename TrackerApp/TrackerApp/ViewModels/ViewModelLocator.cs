using CommonServiceLocator;

namespace TrackerApp.ViewModels
{
    public static class ViewModelLocator
    {
        static ViewModelLocator()
        {
            Bootstrapper.Initialize();
        }

        public static MainPageViewModel MainPageViewModel => ServiceLocator.Current.GetInstance<MainPageViewModel>();
    }
}
