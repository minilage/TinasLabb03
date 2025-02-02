using System.Windows;
using System.Windows.Input;
using TinasLabb03.ViewModel;

namespace TinasLabb03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel = new MainWindowViewModel();

            Loaded += MainWindow_Loaded;
            //Closed += MainWindow_Closed;
        }

        private async void MainWindow_Loaded(object? sender, RoutedEventArgs? e)
        {
            if (viewModel != null)
            {
                // Laddar data när fönstret öppnas
                await viewModel.LoadDataAsync();
            }
            else
            {
                Console.WriteLine("ViewModel är null vid laddning av data.");
            }
        }

        //private async void MainWindow_Closed(object? sender, EventArgs? e)
        //{
        //    if (viewModel != null)
        //    {
        //        // Sparar data när fönstret stängs
        //        await viewModel.SaveDataAsync();
        //    }
        //    else
        //    {
        //        Console.WriteLine("ViewModel är null vid sparning av data.");
        //    }
        //}

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        // Hantering av ESC-tangenten
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (WindowState == WindowState.Maximized) // Kontrollera om vi är i helskärmsläge
                {
                    ExitFullScreen(); // Lämna helskärmsläge
                }
            }
        }

        private void ExitFullScreen()
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;

            if (DataContext is MainWindowViewModel viewModel)
            {
                // Återställ skalan för PlayerViewModel om den är aktiv
                if (viewModel.CurrentView is PlayerViewModel playerViewModel)
                {
                    playerViewModel.AdjustScaleForFullscreen(false);
                }

                System.Console.WriteLine("Lämnar helskärmsläge.");
            }
        }
    }
}
