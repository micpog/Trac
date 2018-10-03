using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Position = TrackerApp.Models.Position;

namespace TrackerApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.BindingContext = new MainPageViewModel();
            InitializeComponent();
        }
    }
}
