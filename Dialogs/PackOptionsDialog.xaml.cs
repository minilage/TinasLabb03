using System.Windows;
using System.Windows.Input;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Dialogs
{
    /// <summary>
    /// Interaction logic for PackOptionsDialog.xaml
    /// </summary>
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog(QuestionPackViewModel activePack)
        {
            InitializeComponent();

            // Sätt DataContext till det aktuella ActivePack
            DataContext = activePack;
        }

        // Hanterar "Save"-knappen
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indikerar att användaren vill spara ändringar
            Close(); // Stänger dialogen
        }

        // Hanterar "Cancel"-knappen
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indikerar att användaren vill avbryta ändringar
            Close(); // Stänger dialogen
        }

        // Hanterar "Close"-kommandot (ESC-tangent)
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false; // Standard för avbrytande
            Close();
        }
    }
}