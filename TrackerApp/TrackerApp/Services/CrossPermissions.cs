using Plugin.Permissions.Abstractions;

namespace TrackerApp.Services
{
    public static class CrossPermissions
    {
        private static IPermissions _permissions;
        public static IPermissions Permissions
        {
            get => _permissions ?? (_permissions = Plugin.Permissions.CrossPermissions.Current);
            set => _permissions = value;
        }
    }
}
