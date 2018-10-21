using System.Threading.Tasks;

namespace TrackerApp.Services
{
    public interface IDialogService
    {
        Task DisplayAlert(string title, string message, string accept);
        Task DisplayAlert(string title, string message, string accept, string cancel);
    }
}