using System.Windows;
using System.Windows.Input;
using TinasLabb03.Model;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Dialogs
{
    // Detta fönster används för att redigera inställningarna för ett befintligt frågepaket.

    public partial class PackOptionsDialog : Window
    {
        private ConfigurationViewModel? configurationViewModel;

        /// <summary>
        /// Konstruktor som sätter DataContext med Pack, Difficulties och Categories.
        /// Här skickas även en lista med tillgängliga kategorier.
        /// Om du inte vill redigera kategorier här, kan du skicka in null eller en tom lista.
        /// </summary>

        public PackOptionsDialog(QuestionPackViewModel activePack, IEnumerable<Difficulty> difficulties, IEnumerable<Category>? categories, int timeLimitInSeconds)
        {
            InitializeComponent();
            // Om ingen lista med kategorier skickas in, använd en tom lista.
            categories ??= new List<Category>();
            //{
            //    Pack = activePack,
            //};
        }

        public PackOptionsDialog(ConfigurationViewModel configurationViewModel)
        {
            this.configurationViewModel = configurationViewModel;
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