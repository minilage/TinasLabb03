﻿using System.Collections.ObjectModel;
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

        private int _score;

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentQuestion => mainWindowViewModel.ActivePack?.Questions[_currentQuestionIndex].Query ?? "No question available";
        public string QuestionInfo => $"Question {_currentQuestionIndex + 1} of {mainWindowViewModel.ActivePack?.Questions.Count ?? 0}";
        public string TimeLeft => $"{_timeLeft} seconds";
        public ObservableCollection<AnswerOptionViewModel> Options { get; set; }


        public bool IsTimeOut
        {
            get => _isTimeOut;
            private set
            {
                _isTimeOut = value;
                RaisePropertyChanged();
            }
        }

        private bool _isQuestionActive;

        public bool IsQuestionActive
        {
            get => _isQuestionActive;
            set
            {
                _isQuestionActive = value;
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
            Options = new ObservableCollection<AnswerOptionViewModel>();

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
            if (mainWindowViewModel.ActivePack == null || _currentQuestionIndex >= mainWindowViewModel.ActivePack.Questions.Count)
            {
                Options = new ObservableCollection<AnswerOptionViewModel>
        {
            new AnswerOptionViewModel("N/A", false),
            new AnswerOptionViewModel("N/A", false),
            new AnswerOptionViewModel("N/A", false),
            new AnswerOptionViewModel("N/A", false)
        };
                RaisePropertyChanged(nameof(Options));
                return;
            }

            IsQuestionActive = true; // Aktivera frågan

            var currentQuestion = mainWindowViewModel.ActivePack.Questions[_currentQuestionIndex];
            Options = new ObservableCollection<AnswerOptionViewModel>(
                currentQuestion.IncorrectAnswers
                    .Select(answer => new AnswerOptionViewModel(answer, false))
                    .Append(new AnswerOptionViewModel(currentQuestion.CorrectAnswer, true))
                    .OrderBy(_ => Guid.NewGuid()) // Blanda alternativen
            );

            RaisePropertyChanged(nameof(Options));
            RaisePropertyChanged(nameof(CurrentQuestion));
            RaisePropertyChanged(nameof(QuestionInfo));
        }

        private void HandleAnswer(object? parameter)
        {
            if (parameter is not AnswerOptionViewModel selectedOption || mainWindowViewModel.ActivePack == null)
                return;

            // Markera rätt och fel alternativ
            foreach (var option in Options)
            {
                // Om användaren tryckt på alternativet
                if (option == selectedOption)
                {
                    option.IsSelected = true; // Markera som valt
                    option.IsCorrect = option.IsCorrect; // Behåll korrekthetsstatus
                }
                else
                {
                    // Markera som ej valt och endast rätt alternativ grönt
                    option.IsSelected = option.IsCorrect;
                }
            }

            // Uppdatera poäng om svaret är rätt
            if (selectedOption.IsCorrect)
            {
                Score++;
            }

            // Uppdatera UI-bindningar
            RaisePropertyChanged(nameof(Score));
            RaisePropertyChanged(nameof(Options));

            // Kontrollera om detta är sista frågan
            if (_currentQuestionIndex + 1 >= mainWindowViewModel.ActivePack.Questions.Count)
            {
                // Visa popup om alla frågor är besvarade
                ShowResultPopup();
            }
            else
            {
                // Vänta 2 sekunder innan nästa fråga
                Task.Delay(2000).ContinueWith(_ =>
                {
                    _currentQuestionIndex++;
                    LoadQuestion();
                }, TaskScheduler.FromCurrentSynchronizationContext());
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
                ScaleFactor = 1.5;
                ButtonFontSize = 24;
                ButtonHeight = 90;
                QuestionFontSize = 36;
            }
            else
            {
                ScaleFactor = 1.0;
                ButtonFontSize = 18;
                ButtonHeight = 60;
                QuestionFontSize = 28;
            }

            RaisePropertyChanged(nameof(ScaleFactor));
            RaisePropertyChanged(nameof(ButtonFontSize));
            RaisePropertyChanged(nameof(ButtonHeight));
            RaisePropertyChanged(nameof(QuestionFontSize));
        }

        private void ShowResultPopup()
        {
            MessageBox.Show($"You got {Score} out of {mainWindowViewModel.ActivePack?.Questions.Count} correct!",
                "Quiz Results", MessageBoxButton.OK);
            QuitToMainMenu(null);
        }
    }
}
