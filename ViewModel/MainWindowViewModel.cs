using System.Collections.ObjectModel;
using System.Windows;
using TinasLabb03.Command;
using TinasLabb03.Data;
using TinasLabb03.Dialogs;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;
        private object? _currentView;

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();

        public static IEnumerable<Difficulty> Difficulties => Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>();

        // Exponerar det aktiva frågepaketet
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

        // Exponerar aktuell vy (t.ex. konfiguration eller spel)
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

        // MongoDB-repository
        private readonly MongoContext _mongoContext;
        private readonly QuestionPackRepository _questionPackRepo;
        private readonly CategoryRepository _categoryRepo;

        public MainWindowViewModel()
        {
            // Använd den parameterlösa MongoContext-konstruktorn med hårdkodade värden
            _mongoContext = new MongoContext();
            _questionPackRepo = new QuestionPackRepository(_mongoContext);
            _categoryRepo = new CategoryRepository(_mongoContext);

            // Initiera konfiguration och sätt aktuell vy
            ConfigurationViewModel = new ConfigurationViewModel(this);
            CurrentView = ConfigurationViewModel;

            // Initiera kommandon för olika åtgärder
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

            // Ladda data asynkront vid start
            Task.Run(LoadDataAsync);
        }

        // Exempel på async-metod för att skapa ett nytt pack.
        private async Task CreatePackAsync()
        {
            // Skapa en ny QuestionPack med defaultkategori "General"
            var newPack = new QuestionPack("New Pack", Difficulty.Medium, 30, "General");
            var newPackViewModel = new QuestionPackViewModel(newPack);

            // Hämta kategorier från databasen via CategoryRepository
            var categories = await _categoryRepo.GetAllAsync();

            // Om inga kategorier finns, skapa en defaultkategori "General"
            if (!categories.Any())
            {
                var defaultCategory = new Category("General");
                await _categoryRepo.AddAsync(defaultCategory);
                categories = new List<Category> { defaultCategory };
            }

            // Skapa dialogen och skicka med listan med kategorier
            var dialog = new CreateNewPackDialog(newPackViewModel, Difficulties, categories);
            if (dialog.ShowDialog() == true)
            {
                await _questionPackRepo.AddAsync(newPack);
                Packs.Add(newPackViewModel);
                ActivePack = newPackViewModel;
            }
        }

        // Skapar flera kategorier i en initialiseringsmetod
        private async Task InitializeCategoriesAsync()
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

        // Tar bort det aktiva frågepaketet från MongoDB och den lokala samlingen.
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

        // Lägger till en ny fråga i det aktiva paketet och uppdaterar MongoDB.
        private void AddQuestion(object? obj)
        {
            if (ActivePack != null)
            {
                var newQuestion = new Question("New Question", "", new[] { "", "", "" }, Difficulty.Medium);
                var newQuestionViewModel = new QuestionViewModel(newQuestion);
                ActivePack.Questions.Add(newQuestionViewModel);
                _ = _questionPackRepo.UpdateAsync(ConvertToModel(ActivePack));
            }
        }

        // Tar bort en fråga från det aktiva paketet och uppdaterar MongoDB.
        private void RemoveQuestion(object? obj)
        {
            if (ActivePack != null && ActivePack.Questions.Any())
            {
                var questionToRemove = ActivePack.Questions.LastOrDefault();
                if (questionToRemove != null)
                {
                    ActivePack.Questions.Remove(questionToRemove);
                    _ = _questionPackRepo.UpdateAsync(ConvertToModel(ActivePack));
                }
            }
        }

        private bool CanModifyPack(object? obj) => ActivePack != null;

        // Öppnar en dialog för att redigera inställningar för det aktiva paketet.
        // Vid spara uppdateras paketet i MongoDB.
        private void OpenPackOptions(object? obj)
        {
            if (ActivePack != null)
            {
                var dialog = new PackOptionsDialog(ActivePack);
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    RaisePropertyChanged(nameof(ActivePack));
                    _ = _questionPackRepo.UpdateAsync(ConvertToModel(ActivePack));
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

        // Laddar alla QuestionPacks från MongoDB och uppdaterar den lokala samlingen.
        public async Task LoadDataAsync()
        {
            try
            {
                var packs = await _questionPackRepo.GetAllAsync();

                // Om inga packs finns, skapa ett default pack
                if (!packs.Any())
                {
                    var defaultPack = new QuestionPack("Default Pack", Difficulty.Medium, 30, "General");
                    await _questionPackRepo.AddAsync(defaultPack);
                    packs = new System.Collections.Generic.List<QuestionPack> { defaultPack };
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

        // Konverterar ett QuestionPackViewModel till dess modellvariant för att spara i databasen.
        private QuestionPack ConvertToModel(QuestionPackViewModel vm)
        {
            var pack = new QuestionPack(
                vm.Name,
                vm.Difficulty,
                vm.TimeLimitInSeconds,
                vm.Category
            );
            pack.Id = vm.Id; // Kopiera Id om det finns
            pack.Questions = new ObservableCollection<Question>(
                vm.Questions.Select(qvm => new Question(
                    qvm.Query,
                    qvm.CorrectAnswer,
                    qvm.Options.Select(o => o.Text).ToArray(),
                    qvm.Difficulty
                ))
            );
            return pack;
        }
    }
}
// SaveDataAsync används ej då databasen uppdateras direkt via repository.
//public async Task SaveDataAsync()
//{
//    try
//    {
//        var packs = Packs.Select(p =>
//        {
//            var pack = new QuestionPack(
//                p.Name,
//                p.Difficulty,
//                p.TimeLimitInSeconds,
//                p.Category ?? "DefaultCategory"
//            );
//            pack.Questions = new ObservableCollection<Question>(p.Questions.Select(q => new Question(
//                q.Query,
//                q.CorrectAnswer,
//                q.Options.Where(option => option.Text != q.CorrectAnswer)
//                         .Select(option => option.Text)
//                         .ToArray(),
//                q.Difficulty
//            )));
//            return pack;
//        }).ToArray();

//        string json = JsonSerializer.Serialize(packs, JsonOptions);
//        await File.WriteAllTextAsync(FilePath, json);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error saving data: {ex.Message}");
//    }
//}