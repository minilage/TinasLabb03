using TinasLabb03.Command;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private Question? _selectedQuestion;
        private bool _isRightPanelVisible;

        public static IEnumerable<Difficulty> Difficulties => Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

        public ConfigurationViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));
            IsRightPanelVisible = false; // Högerpanelen är gömd som standard

            AddQuestionCommand = new DelegateCommand(AddQuestion, CanModifyPack);
            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, CanModifyPack);
            PackOptionsCommand = new DelegateCommand(OpenPackOptions, CanModifyPack);
        }

        public QuestionPackViewModel? ActivePack => mainWindowViewModel.ActivePack;

        public Question? SelectedQuestion
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
                ActivePack.Questions.Add(newQuestion);
                SelectedQuestion = newQuestion;
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
                var dialog = new PackOptionsDialog(ActivePack);
                if (dialog.ShowDialog() == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                }
            }
        }

        private bool CanModifyPack(object? obj) => ActivePack != null;
    }
}
