using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using TrackerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = ViewModelLocator.MainPageViewModel;
        }
    }
}
