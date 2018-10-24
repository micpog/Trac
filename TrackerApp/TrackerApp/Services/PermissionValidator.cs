using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace TrackerApp.Services
{
    public class PermissionValidator : IPermissionValidator
    {
        private readonly IDialogService _dialogService;

        public PermissionValidator(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> ValidateGeolocationPermission()
        {
            try
            {
                var current = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (current != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location);

                    var all = await CrossPermissions.Current.RequestPermissionsAsync(new[]
                        {Permission.Location});
                    current = all[Permission.Location];
                }

                if (current == PermissionStatus.Granted)
                {
                    return true;
                }

                await _dialogService.DisplayAlert("No permission granted",
                    "Location is required for GPS tracking.", "Ok");
                return false;
            }
            catch (Exception e)
            {
                await _dialogService.DisplayAlert("Something wrong with permission granting", e.Message, "Ok");
                throw;
            }
        }

    }
}