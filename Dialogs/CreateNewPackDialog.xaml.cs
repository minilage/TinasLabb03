using System.Windows;
using TinasLabb03.Model;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Dialogs
{
    public partial class CreateNewPackDialog : Window
    {
        internal CreateNewPackDialog(QuestionPackViewModel packViewModel, IEnumerable<Difficulty> difficulties)
        {
            InitializeComponent();

            // Sätt DataContext för dialogen
            DataContext = new
            {
                Pack = packViewModel,
                Difficulties = difficulties
            };
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indikerar att användaren vill skapa ett nytt pack
            Close(); // Stänger dialogen
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indikerar att användaren vill avbryta
            Close(); // Stänger dialogen
        }

        private void CloseCommandHandler(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = false; // Standard för avbrytande
            Close();
        }
    }
}
