using System.Windows;
using System.Windows.Input;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Dialogs
{
    // Interaction logic for PackOptionsDialog.xaml.
    // Detta fönster används för att redigera inställningarna för ett befintligt frågepaket.

    public partial class PackOptionsDialog : Window
    {
        // Konstruktor som sätter DataContext till det aktiva packet.
        // activePack - det QuestionPackViewModel som ska redigeras.
        public PackOptionsDialog(QuestionPackViewModel activePack)

        {
            InitializeComponent();
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

        // Hanterar "Close"-kommandot (t.ex. via Esc-tangent)
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false; // Standard för avbrytande
            Close();
        }
    }
}