using System.Threading.Tasks;

namespace TrackerApp.Services
{
    public interface IPermissionValidator
    {
        Task<bool> ValidateGeolocationPermission();
    }
}