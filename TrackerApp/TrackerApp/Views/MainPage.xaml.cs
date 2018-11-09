using TrackerApp.ViewModels;
using Xamarin.Forms;

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
