using System.Windows;
using TinasLabb03.Model;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Dialogs
{

    // Interaction logic for CreateNewPackDialog.xaml.
    // Detta fönster används för att skapa ett nytt frågepaket.

    public partial class CreateNewPackDialog : Window
    {
        // Konstruktor som sätter DataContext med Pack och en lista med Difficulties.

        public CreateNewPackDialog(QuestionPackViewModel packViewModel, IEnumerable<Difficulty> difficulties, IEnumerable<Category> categories)
        {
            InitializeComponent();
            // Sätter DataContext som ett anonymt objekt med egenskaperna Pack och Difficulties
            DataContext = new
            {
                Pack = packViewModel,
                Difficulties = difficulties,
                Categories = categories
            };
        }

        // Hanterar Create-knappens klickhändelse.
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indikerar att användaren vill skapa ett nytt pack
            Close(); // Stänger dialogen
        }

        // Hanterar Cancel-knappens klickhändelse.
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indikerar att användaren vill avbryta
            Close(); // Stänger dialogen
        }

        // Hanterar Close-kommandot (t.ex. via Esc-tangent).
        private void CloseCommandHandler(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = false; // Standard för avbrytande
            Close();
        }
    }
}