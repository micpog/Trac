using Plugin.Geolocator.Abstractions;

namespace TrackerApp.Services
{
    public static class CrossGeolocator
    {
        private static IGeolocator _crossGeolocator;
        public static IGeolocator Geolocator
        {
            get => _crossGeolocator ?? (_crossGeolocator = Plugin.Geolocator.CrossGeolocator.Current);
            set => _crossGeolocator = value;
        }
    }
}
