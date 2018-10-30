using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace TrackerApp.Services
{
    public class PermissionValidator : IPermissionValidator
    {
        private readonly IDialogService _dialogService;
        private readonly IPermissions _permission = CrossPermissions.Permissions;

        public PermissionValidator(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> ValidateGeolocationPermission()
        {
            try
            {
                var current = await _permission.CheckPermissionStatusAsync(Permission.Location);
                if (current != PermissionStatus.Granted)
                {
                    await _permission.ShouldShowRequestPermissionRationaleAsync(Permission.Location);

                    var all = await _permission.RequestPermissionsAsync(new[]
                        {Permission.Location});
                    current = all[Permission.Location];
                }

                if (current == PermissionStatus.Granted)
                {
                    return true;
                }

                _dialogService.DisplayAlert("No permission granted",
                    "Location is required for GPS tracking.", "Ok").RunSynchronously();
                return false;
            }
            catch (Exception e)
            {
                _dialogService.DisplayAlert("Something wrong with permission granting", e.Message, "Ok").RunSynchronously();
                throw;
            }
        }

    }
}