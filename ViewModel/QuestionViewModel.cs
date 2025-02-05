using System.Collections.ObjectModel;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    // ViewModel för en enskild fråga.
    public class QuestionViewModel : ViewModelBase
    {
        private readonly Question model;
        private string? _selectedAnswer;

        // Lista med felaktiga svar (enkel stränglista)
        public ObservableCollection<string> IncorrectAnswers { get; }
        // Lista med svarsalternativ som ViewModels
        public ObservableCollection<AnswerOptionViewModel> Options { get; }

        // Det valda svarsalternativet, notifierar UI:t vid ändring.
        public string SelectedAnswer
        {
            get => _selectedAnswer!;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsCorrect));
                    RaisePropertyChanged(nameof(IsIncorrect));
                }
            }
        }

        // Booleska egenskaper för att indikera om det valda svaret är korrekt/inkorrekt
        public bool IsCorrect => SelectedAnswer == CorrectAnswer;
        public bool IsIncorrect => SelectedAnswer != null && SelectedAnswer != CorrectAnswer;

        public string Query
        {
            get => model.Query;
            set
            {
                if (model.Query != value)
                {
                    model.Query = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CorrectAnswer
        {
            get => model.CorrectAnswer;
            set
            {
                if (model.CorrectAnswer != value)
                {
                    model.CorrectAnswer = value;
                    RaisePropertyChanged();
                    UpdateOptions();
                }
            }
        }

        public Difficulty Difficulty
        {
            get => model.Difficulty;
            set
            {
                if (model.Difficulty != value)
                {
                    model.Difficulty = value;
                    RaisePropertyChanged();
                }
            }
        }

        public QuestionViewModel(Question model)
        {
            this.model = model;
            IncorrectAnswers = new ObservableCollection<string>(model.IncorrectAnswers);
            Options = new ObservableCollection<AnswerOptionViewModel>();
            UpdateOptions();

            // När felaktiga svar ändras, uppdatera modellen och UI:t
            IncorrectAnswers.CollectionChanged += (sender, args) =>
            {
                model.IncorrectAnswers = IncorrectAnswers.ToArray();
                RaisePropertyChanged(nameof(IncorrectAnswers));
                UpdateOptions();
            };
        }

        // Uppdaterar listan med svarsalternativ genom att lägga till de 
        // felaktiga och korrekta svaren och blandar dem slumpmässigt.
        private void UpdateOptions()
        {
            Options.Clear();

            // Lägg till felaktiga svar
            foreach (var incorrectAnswer in model.IncorrectAnswers)
            {
                Options.Add(new AnswerOptionViewModel(incorrectAnswer, false));
            }

            // Lägg till det korrekta svaret
            Options.Add(new AnswerOptionViewModel(model.CorrectAnswer, true));

            // // Blanda alternativen slumpmässigt
            var shuffledOptions = Options.OrderBy(_ => Guid.NewGuid()).ToList();
            Options.Clear();
            foreach (var option in shuffledOptions)
            {
                Options.Add(option);
            }

            RaisePropertyChanged(nameof(Options));
        }
    }
}