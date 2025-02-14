using TinasLabb03.Command;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    // ViewModel för konfigurationen av frågepaket, hanterar redigering av frågor och packinställningar.

    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private QuestionViewModel? _selectedQuestion;
        private bool _isRightPanelVisible;

        public static IEnumerable<Difficulty> Difficulties => Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

        public DelegateCommand ManageCategoriesCommand { get; }

        public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
            IsRightPanelVisible = false; // Högerpanelen är gömd som standard

            // Initiera kommandon för att lägga till och ta bort frågor
            AddQuestionCommand = new DelegateCommand(AddQuestion, CanModifyPack);
            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, CanModifyPack);
            PackOptionsCommand = new DelegateCommand(OpenPackOptions, CanModifyPack);

            ManageCategoriesCommand = new DelegateCommand(obj => mainWindowViewModel.OpenCategoryManagement());
        }

        public QuestionPackViewModel? ActivePack => mainWindowViewModel.ActivePack;

        public QuestionViewModel? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                IsRightPanelVisible = value != null; // Visa högerpanelen om en fråga är vald
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsRightPanelVisible));
            }
        }

        public bool IsRightPanelVisible
        {
            get => _isRightPanelVisible;
            set
            {
                _isRightPanelVisible = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AddQuestionCommand { get; }
        public DelegateCommand RemoveQuestionCommand { get; }
        public DelegateCommand PackOptionsCommand { get; }

        private void AddQuestion(object? obj)
        {
            if (ActivePack != null)
            {
                var newQuestion = new Question("New Question", "", new[] { "", "", "" }, Difficulty.Medium);
                var newQuestionViewModel = new QuestionViewModel(newQuestion);
                ActivePack.Questions.Add(newQuestionViewModel);
                SelectedQuestion = newQuestionViewModel;
            }
        }

        private void RemoveQuestion(object? obj)
        {
            if (ActivePack != null && SelectedQuestion != null)
            {
                ActivePack.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
            }
        }

        private void OpenPackOptions(object? obj)
        {
            if (ActivePack != null)
            {
                // Vid öppnande av PackOptionsDialog kan redigering av packinställningar göras
                var dialog = new PackOptionsDialog(ActivePack, Difficulties, null, ActivePack.TimeLimitInSeconds);

                if (dialog.ShowDialog() == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                }
            }
        }

        private bool CanModifyPack(object? obj) => ActivePack != null;
    }
}
