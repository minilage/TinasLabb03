using System.Collections.ObjectModel;
using System.Windows;
using TinasLabb03.Command;
using TinasLabb03.Data;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;
using Application = System.Windows.Application;

namespace TinasLabb03.ViewModel
{
    /// <summary>
    /// Huvud-ViewModel för applikationen. Hanterar navigering och CRUD-operationer via MongoDB.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        // Fält
        private QuestionPackViewModel? _activePack;
        private object? _currentView;

        // Egenskaper
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

        public ConfigurationViewModel ConfigurationViewModel { get; }

        // Kommandon
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
        public DelegateCommand ManageCategoriesCommand { get; }

        // MongoDB-repository
        private readonly MongoContext _mongoContext;
        private readonly QuestionPackRepository _questionPackRepo;
        private readonly CategoryRepository _categoryRepo;

        // Konstruktor
        public MainWindowViewModel()
        {
            // Initiera MongoContext med hårdkodade värden
            _mongoContext = new MongoContext();
            _questionPackRepo = new QuestionPackRepository(_mongoContext);
            _categoryRepo = new CategoryRepository(_mongoContext);

            ConfigurationViewModel = new ConfigurationViewModel(this);
            CurrentView = ConfigurationViewModel;

            // Initiera kommandon
            CreatePackCommand = new DelegateCommand(async obj => await CreatePackAsync());
            DeletePackCommand = new DelegateCommand(async obj => await DeleteActivePackAsync(), CanDeletePack);
            AddQuestionCommand = new DelegateCommand(obj => AddQuestion(obj), CanModifyPack);
            RemoveQuestionCommand = new DelegateCommand(obj => RemoveQuestion(obj), CanModifyPack);
            PackOptionsCommand = new DelegateCommand(obj => OpenPackOptions(obj), CanModifyPack);
            ExitCommand = new DelegateCommand(obj => ExitApplication(obj));
            FullScreenCommand = new DelegateCommand(obj => ToggleFullScreen(obj));
            SelectPackCommand = new DelegateCommand(obj => SelectPack(obj));

            GoToConfigurationCommand = new DelegateCommand(_ =>
            {
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
                    MessageBox.Show("No active question pack selected. Please select a pack before starting the game.",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
            ManageCategoriesCommand = new DelegateCommand(obj => OpenCategoryManagement());

            // Ladda data från databasen vid start
            Task.Run(LoadDataAsync);
        }

        /// <summary>
        /// Initierar default-kategorier om de inte redan finns.
        /// </summary>
        private async Task InitializeCategoriesAsync()
        {
            try
            {
                var existingCategories = await _categoryRepo.GetAllAsync();
                if (!existingCategories.Any())
                {
                    var categoriesToAdd = new List<Category>
                {
                    new Category("General"),
                    new Category("Entertainment"),
                    new Category("Science"),
                    new Category("Geography"),
                    new Category("History"),
                    new Category("Pop Culture")
                };

                    foreach (var cat in categoriesToAdd)
                    {
                        await _categoryRepo.AddAsync(cat);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in InitializeCategoriesAsync:");
            }
        }

        /// <summary>
        /// Skapar ett default-questionpack med två exempel-frågor och sparar det i databasen.
        /// </summary>
        private async Task CreateDefaultPackAsync()
        {
            var defaultPack = new QuestionPack("Default Pack", Difficulty.Medium, 30, "General");

            var question1 = new Question(
                "Vad är huvudstaden i Frankrike?",
                "Paris",
                new string[] { "Berlin", "Madrid", "Rom" },
                Difficulty.Medium
            );
            var question2 = new Question(
                "Vilket ämne studerar man i kemi?",
                "Kemiska reaktioner",
                new string[] { "Historia", "Fysik", "Biologi" },
                Difficulty.Easy
            );

            defaultPack.Questions.Add(question1);
            defaultPack.Questions.Add(question2);

            await _questionPackRepo.AddAsync(defaultPack);

            var defaultPackVM = new QuestionPackViewModel(defaultPack);
            Packs.Add(defaultPackVM);
            ActivePack = defaultPackVM;
        }

        /// <summary>
        /// Laddar alla QuestionPacks från databasen. Om inga finns, skapas ett default-pack.
        /// </summary>
        public async Task LoadDataAsync()
        {
            try
            {
                // Säkerställ att kategorierna finns
                await InitializeCategoriesAsync();

                var packs = await _questionPackRepo.GetAllAsync();
                if (!packs.Any() || !packs.Any(p => p.Name == "Default Pack"))
                {
                    await CreateDefaultPackAsync();
                    packs = await _questionPackRepo.GetAllAsync();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Packs.Clear();
                    foreach (var pack in packs)
                    {
                        Packs.Add(new QuestionPackViewModel(pack));
                    }
                    ActivePack = Packs.FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from MongoDB: {ex.Message}");
            }
        }

        /// <summary>
        /// Skapar ett nytt Question Pack via dialog.
        /// Hämtar kategorier från databasen och skickar med dem till dialogen.
        /// </summary>

        private async Task CreatePackAsync()
        {
            var newPack = new QuestionPack("New Pack", Difficulty.Medium, 30, "General");
            var newPackViewModel = new QuestionPackViewModel(newPack);

            var categories = await _categoryRepo.GetAllAsync();
            if (!categories.Any())
            {
                var defaultCategory = new Category("General");
                await _categoryRepo.AddAsync(defaultCategory);
                categories = new List<Category> { defaultCategory };
            }

            // Skapa dialogen med Pack, Difficulties och Categories
            var dialog = new CreateNewPackDialog(newPackViewModel, Difficulties, categories);
            if (dialog.ShowDialog() == true)
            {
                await _questionPackRepo.AddAsync(newPack);
                Packs.Add(newPackViewModel);
                ActivePack = newPackViewModel;
            }
        }

        /// <summary>
        /// Tar bort det aktiva Question Pack från databasen.
        /// </summary>
        private async Task DeleteActivePackAsync()
        {
            if (ActivePack != null)
            {
                await _questionPackRepo.DeleteAsync(ActivePack.Id!);
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

        /// <summary>
        /// Öppnar PackOptionsDialog för att redigera inställningar för det aktiva packet.
        /// Skickar med Difficulties och en lista med kategorier (här hämtas från databasen).
        /// </summary>
        private async void OpenPackOptions(object? obj)
        {
            if (ActivePack != null)
            {
                var categories = await _categoryRepo.GetAllAsync();
                var dialog = new PackOptionsDialog(ActivePack, Difficulties, categories);
                if (dialog.ShowDialog() == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                    // Du kan lägga till en uppdateringsmetod här om packet ska sparas
                }
            }
        }

        /// <summary>
        /// Öppnar CategoryManagementDialog så att användaren kan lägga till/ta bort kategorier.
        /// </summary>

        public void OpenCategoryManagement()
        {
            var dialog = new CategoryManagementDialog(_categoryRepo);
            dialog.ShowDialog();
        }

        // Applikationshantering
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

        // Stub för SaveDataAsync – ej nödvändig då data sparas direkt via repository
        public async Task SaveDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}