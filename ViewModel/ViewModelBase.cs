using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TinasLabb03.ViewModel
{
    /// <summary>
    /// Bas-klass för alla ViewModels som implementerar INotifyPropertyChanged.
    /// Används för att notifiera UI:t vid förändringar.
    /// </summary>

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// Metod för att höja PropertyChanged-händelsen.
        public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
