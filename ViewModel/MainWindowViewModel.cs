using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using TinasLabb03.Command;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace TinasLabb03.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        // ===========================
        //  Fält & Konstanter
        // ===========================

        private QuestionPackViewModel? _activePack;
        private object? _currentView;
        private bool _configMode = true;
        private bool _playMode = false;

        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TinasLabb03");
        private static readonly string FilePath = Path.Combine(AppFolder, "QuestionPacks.json");
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        // ===========================
        //  Egenskaper
        // ===========================

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();
        public static IEnumerable<Difficulty> Difficulties => Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                ConfigurationViewModel.RaisePropertyChanged(nameof(ActivePack));
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

        public bool PlayMode
        {
            get => _playMode;
            private set
            {
                _playMode = value;
                RaisePropertyChanged();
            }
        }

        public bool ConfigMode
        {
            get => _configMode;
            private set
            {
                _configMode = value;
                RaisePropertyChanged();
            }
        }

        public ConfigurationViewModel ConfigurationViewModel { get; }

        // ===========================
        //  Kommandon
        // ===========================

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

        // ===========================
        //  Konstruktor
        // ===========================

        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            CurrentView = ConfigurationViewModel;

            // Initiera kommandon
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
                if (PlayMode)
                {
                    PlayMode = false;
                    ConfigMode = true;
                    CurrentView = ConfigurationViewModel;
                }
            });

            GoToPlayerCommand = new DelegateCommand(_ =>
            {
                if (ActivePack != null && !PlayMode)
                {
                    var playerViewModel = new PlayerViewModel(this);
                    playerViewModel.StartGame(ActivePack);
                    CurrentView = playerViewModel;
                    ConfigMode = false;
                    PlayMode = true;
                }
                else
                {
                    MessageBox.Show("No active question pack selected. Please select a pack before starting the game.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            Task.Run(LoadDataAsync);
        }

        // ===========================
        //  Metoder: Hantering av Question Packs
        // ===========================

        private void CreatePack(object? obj)
        {
            var newPackViewModel = new QuestionPackViewModel(new QuestionPack("New Pack", Difficulty.Medium, 30));

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
                ActivePack.Questions.Remove(ActivePack.Questions.Last());
            }
        }

        private bool CanModifyPack(object? obj) => ActivePack != null;

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

        // ===========================
        //  Metoder: Applikationshantering
        // ===========================

        private void ExitApplication(object? obj) => Application.Current.Shutdown();

        private void ToggleFullScreen(object? obj)
        {
            if (Application.Current.MainWindow is { } mainWindow)
            {
                bool isFullscreen = mainWindow.WindowState == WindowState.Normal;
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

        // ===========================
        //  Metoder: Ladda & Spara Data
        // ===========================

        public async Task LoadDataAsync()
        {
            try
            {
                Directory.CreateDirectory(AppFolder);
                if (!File.Exists(FilePath))
                    await File.WriteAllTextAsync(FilePath, "[]");

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
                string json = JsonSerializer.Serialize(Packs.Select(p => p.ToModel()), JsonOptions);
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
