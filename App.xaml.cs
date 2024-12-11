using System.Windows;
using TinasLabb03.ViewModel;

namespace TinasLabb03
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnExit(ExitEventArgs e)
        {
            // Kontrollera om MainWindow har en DataContext av typen MainWindowViewModel
            if (MainWindow?.DataContext is MainWindowViewModel mainWindowViewModel)
            {
                // Spara data asynkront
                await mainWindowViewModel.SaveDataAsync();
            }

            // Anropa basimplementationen av OnExit
            base.OnExit(e);
        }
    }
}
