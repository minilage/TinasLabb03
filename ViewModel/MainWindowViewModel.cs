using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using TinasLabb03.Command;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;
        private object? _currentView;

        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TinasLabb03");
        private static readonly string FilePath = Path.Combine(AppFolder, "QuestionPacks.json");
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();

        public static IEnumerable<Difficulty> Difficulties => Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                ConfigurationViewModel.RaisePropertyChanged("ActivePack");
                RaisePropertyChanged();
            }
        }

        public object? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                RaisePropertyChanged();
            }
        }

        public ConfigurationViewModel ConfigurationViewModel { get; }
        public DelegateCommand CreatePackCommand { get; }
        public DelegateCommand DeletePackCommand { get; }
        public DelegateCommand AddQuestionCommand { get; }
        public DelegateCommand RemoveQuestionCommand { get; }
        public DelegateCommand PackOptionsCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand FullScreenCommand { get; }
        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand GoToConfigurationCommand { get; }
        public DelegateCommand GoToPlayerCommand { get; }

        public MainWindowViewModel()
        {
            using (var db = new SaveDBContext())
            {
                db.Database.EnsureCreated();
            }

            ConfigurationViewModel = new ConfigurationViewModel(this);
            CurrentView = ConfigurationViewModel;

            CreatePackCommand = new DelegateCommand(CreatePack);
            DeletePackCommand = new DelegateCommand(DeleteActivePack, CanDeletePack);
            AddQuestionCommand = new DelegateCommand(AddQuestion, CanModifyPack);
            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, CanModifyPack);
            PackOptionsCommand = new DelegateCommand(OpenPackOptions, CanModifyPack);
            ExitCommand = new DelegateCommand(ExitApplication);
            FullScreenCommand = new DelegateCommand(ToggleFullScreen);
            SelectPackCommand = new DelegateCommand(SelectPack);

            GoToConfigurationCommand = new DelegateCommand(_ =>
            {
                if (CurrentView is PlayerViewModel playerViewModel)
                {
                    playerViewModel.AdjustScaleForFullscreen(false);
                }
                CurrentView = ConfigurationViewModel;
            });

            GoToPlayerCommand = new DelegateCommand(_ =>
            {
                if (ActivePack != null)
                {
                    var playerViewModel = new PlayerViewModel(this);
                    playerViewModel.StartGame(ActivePack);
                    CurrentView = playerViewModel;
                }
                else
                {
                    MessageBox.Show("No active question pack selected. Please select a pack before starting the game.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            Task.Run(LoadDataAsync);
        }

        private void CreatePack(object? obj)
        {
            var newPackViewModel = new QuestionPackViewModel(
                new QuestionPack("New Pack", Difficulty.Medium, 30)
            );

            var dialog = new CreateNewPackDialog(newPackViewModel, Difficulties);

            if (dialog.ShowDialog() == true)
            {
                Packs.Add(newPackViewModel);
                ActivePack = newPackViewModel;
            }
        }

        private void DeleteActivePack(object? obj)
        {
            if (ActivePack != null)
            {
                Packs.Remove(ActivePack);
                ActivePack = Packs.FirstOrDefault();
            }
        }

        private bool CanDeletePack(object? obj) => ActivePack != null;

        private void AddQuestion(object? obj)
        {
            if (ActivePack != null)
            {
                var newQuestion = new Question("New Question", "", new[] { "", "", "" }, Difficulty.Medium);
                var newQuestionViewModel = new QuestionViewModel(newQuestion);
                ActivePack.Questions.Add(newQuestionViewModel);
            }
        }

        private void RemoveQuestion(object? obj)
        {
            if (ActivePack != null && ActivePack.Questions.Any())
            {
                var questionToRemove = ActivePack.Questions.LastOrDefault();
                if (questionToRemove != null)
                {
                    ActivePack.Questions.Remove(questionToRemove);
                }
            }
        }

        private bool CanModifyPack(object? obj) => ActivePack != null;

        private void OpenPackOptions(object? obj)
        {
            if (ActivePack != null)
            {
                var dialog = new PackOptionsDialog(ActivePack);
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                }
            }
        }

        private void ExitApplication(object? obj) => Application.Current.Shutdown();

        private void ToggleFullScreen(object? obj)
        {
            if (Application.Current.MainWindow is { } mainWindow)
            {
                var isFullscreen = mainWindow.WindowState == WindowState.Normal;
                mainWindow.WindowState = isFullscreen ? WindowState.Maximized : WindowState.Normal;
                mainWindow.WindowStyle = isFullscreen ? WindowStyle.None : WindowStyle.SingleBorderWindow;

                if (CurrentView is PlayerViewModel playerViewModel)
                {
                    playerViewModel.AdjustScaleForFullscreen(isFullscreen);
                }
            }
        }

        private void SelectPack(object? obj)
        {
            if (obj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                if (!Directory.Exists(AppFolder))
                    Directory.CreateDirectory(AppFolder);

                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "[]");
                }

                string json = await File.ReadAllTextAsync(FilePath);
                var packs = JsonSerializer.Deserialize<QuestionPack[]>(json, JsonOptions) ?? Array.Empty<QuestionPack>();

                Packs.Clear();
                foreach (var pack in packs)
                {
                    Packs.Add(new QuestionPackViewModel(pack));
                }

                ActivePack = Packs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        public async Task SaveDataAsync()
        {
            try
            {
                var packs = Packs.Select(p => new QuestionPack(
                    p.Name,
                    p.Difficulty,
                    p.TimeLimitInSeconds,
                new ObservableCollection<Question>(p.Questions.Select(q => new Question(
                    q.Query,
                    q.CorrectAnswer,
                    q.Options.Where(option => option.Text != q.CorrectAnswer).Select(option => option.Text).ToArray(),
                    q.Difficulty
                )))

                )).ToArray();

                string json = JsonSerializer.Serialize(packs, JsonOptions);
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
