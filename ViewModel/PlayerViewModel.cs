using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using TinasLabb03.Command;

namespace TinasLabb03.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly DispatcherTimer timer;

        private int _currentQuestionIndex;
        private int _timeLeft;
        private bool _isTimeOut;
        private double _scaleFactor = 1.0;
        private double _questionFontSize = 28; // Standardstorlek för frågor
        private double _buttonFontSize = 18;  // Standardstorlek för knapptext
        private double _buttonHeight = 60;    // Standardhöjd för knappar

        public string CurrentQuestion => mainWindowViewModel.ActivePack?.Questions[_currentQuestionIndex].Query ?? "No question available";
        public string QuestionInfo => $"Question {_currentQuestionIndex + 1} of {mainWindowViewModel.ActivePack?.Questions.Count ?? 0}";
        public string TimeLeft => $"{_timeLeft} seconds";
        public ObservableCollection<string> Options { get; set; }

        public bool IsTimeOut
        {
            get => _isTimeOut;
            private set
            {
                _isTimeOut = value;
                RaisePropertyChanged();
            }
        }

        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = value;
                RaisePropertyChanged();
            }
        }

        public double QuestionFontSize
        {
            get => _questionFontSize;
            set
            {
                _questionFontSize = value;
                RaisePropertyChanged();
            }
        }

        public double ButtonFontSize
        {
            get => _buttonFontSize;
            set
            {
                _buttonFontSize = value;
                RaisePropertyChanged();
            }
        }

        public double ButtonHeight
        {
            get => _buttonHeight;
            set
            {
                _buttonHeight = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AnswerCommand { get; }
        public DelegateCommand ContinueCommand { get; }
        public DelegateCommand QuitCommand { get; }

        public PlayerViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;

            AnswerCommand = new DelegateCommand(HandleAnswer);
            ContinueCommand = new DelegateCommand(ContinueToNextQuestion);
            QuitCommand = new DelegateCommand(QuitToMainMenu);
            Options = new ObservableCollection<string>() { "", "", "", "" };

            if (mainWindowViewModel.ActivePack != null)
            {
                StartGame(mainWindowViewModel.ActivePack);
            }
        }

        public void StartGame(QuestionPackViewModel activePack)
        {
            if (activePack == null) throw new ArgumentNullException(nameof(activePack));

            _currentQuestionIndex = 0;
            _timeLeft = activePack.TimeLimitInSeconds;
            IsTimeOut = false;

            LoadQuestion();
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timeLeft--;
            RaisePropertyChanged(nameof(TimeLeft));

            if (_timeLeft <= 0)
            {
                timer.Stop();
                IsTimeOut = true;
            }
        }

        private void LoadQuestion()
        {
            // Kontrollera om det finns ett aktivt paket och om indexet är giltigt
            if (mainWindowViewModel.ActivePack == null || _currentQuestionIndex >= mainWindowViewModel.ActivePack.Questions.Count)
            {
                Options = new ObservableCollection<string> { "N/A", "N/A", "N/A", "N/A" };
                RaisePropertyChanged(nameof(Options));
                return;
            }

            // Hämta den aktuella frågan
            var currentQuestion = mainWindowViewModel.ActivePack.Questions[_currentQuestionIndex];

            // Validera att det finns exakt tre felaktiga svar
            if (currentQuestion.Options.Count != 4)
            {
                throw new InvalidOperationException("Each question must have exactly four options.");
            }

            // Skapa en temporär lista för svarsalternativen
            var tempOption = new ObservableCollection<string>();

            // Ladda alternativen
            foreach (var option in currentQuestion.Options)
            {
                tempOption.Add(option);
            }

            // Tilldela tempOption till den bindningsbara Options-egenskapen
            Options = tempOption;

            // Notifiera om att CurrentQuestion, QuestionInfo och Options har ändrats
            RaisePropertyChanged(nameof(CurrentQuestion));
            RaisePropertyChanged(nameof(QuestionInfo));
            RaisePropertyChanged(nameof(Options));
        }


        private void HandleAnswer(object? obj)
        {
            if (obj is int selectedIndex && mainWindowViewModel.ActivePack != null)
            {
                var correctAnswer = mainWindowViewModel.ActivePack.Questions[_currentQuestionIndex].CorrectAnswer;

                if (Options[selectedIndex] == correctAnswer)
                {
                    MessageBox.Show("Correct!");
                }
                else
                {
                    MessageBox.Show("Incorrect!");
                }

                timer.Stop();
                IsTimeOut = true;
            }
        }

        private void ContinueToNextQuestion(object? obj)
        {
            IsTimeOut = false;

            if (_currentQuestionIndex + 1 < (mainWindowViewModel.ActivePack?.Questions.Count ?? 0))
            {
                _currentQuestionIndex++;
                _timeLeft = mainWindowViewModel.ActivePack?.TimeLimitInSeconds ?? 30;
                LoadQuestion();
                timer.Start();
            }
            else
            {
                MessageBox.Show($"Quiz Finished! You answered {_currentQuestionIndex} questions.", "Quiz Finished", MessageBoxButton.OK);
                QuitToMainMenu(null);
            }
        }

        private void QuitToMainMenu(object? obj)
        {
            timer.Stop();
            mainWindowViewModel.CurrentView = mainWindowViewModel.ConfigurationViewModel;
        }

        public void AdjustScaleForFullscreen(bool isFullscreen)
        {
            if (isFullscreen)
            {
                // Öka storlek i fullskärmsläge
                ScaleFactor = 1.5;
                ButtonFontSize = 24;
                ButtonHeight = 90;
                QuestionFontSize = 36;
            }
            else
            {
                // Återställ storlek till normalläge
                ScaleFactor = 1.0;
                ButtonFontSize = 18;
                ButtonHeight = 60;
                QuestionFontSize = 28;
            }

            // Anropa RaisePropertyChanged för att säkerställa att UI uppdateras
            RaisePropertyChanged(nameof(ScaleFactor));
            RaisePropertyChanged(nameof(ButtonFontSize));
            RaisePropertyChanged(nameof(ButtonHeight));
            RaisePropertyChanged(nameof(QuestionFontSize));
        }
    }
}
