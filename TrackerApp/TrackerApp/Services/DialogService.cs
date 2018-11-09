using System.Threading.Tasks;
using Xamarin.Forms;

namespace TrackerApp.Services
{
    public class DialogService : IDialogService
    {
        public async Task DisplayAlert(string title, string message, string accept)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, accept);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}