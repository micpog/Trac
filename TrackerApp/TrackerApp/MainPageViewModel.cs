using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrackerApp.Droid.Annotations;
using TrackerApp.Models;

namespace TrackerApp
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Position> _positions;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel(ObservableCollection<Position> positions)
        {
            this.Positions = positions;
        }

        public ObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (value != null)
                {
                    _positions = value;
                    OnPropertyChanged(nameof(Positions));
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
