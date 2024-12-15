using System.Collections.ObjectModel;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    public class QuestionViewModel : ViewModelBase
    {
        private readonly Question model;

        public ObservableCollection<string> IncorrectAnswers { get; }
        public ObservableCollection<string> Options { get; }

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
                    UpdateOptions(); // Uppdatera alternativen när CorrectAnswer ändras
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

            // Initiera IncorrectAnswers som en ObservableCollection
            IncorrectAnswers = new ObservableCollection<string>(model.IncorrectAnswers);

            // Prenumerera på förändringar i IncorrectAnswers för att uppdatera modellen och Options
            IncorrectAnswers.CollectionChanged += (sender, args) =>
            {
                model.IncorrectAnswers = IncorrectAnswers.ToArray();
                RaisePropertyChanged(nameof(IncorrectAnswers));
                UpdateOptions(); // Uppdatera alternativen när IncorrectAnswers ändras
            };

            // Generera svarsalternativ (Options) baserat på IncorrectAnswers och CorrectAnswer
            Options = new ObservableCollection<string>(
                model.IncorrectAnswers.Append(model.CorrectAnswer).OrderBy(_ => Guid.NewGuid())
            );
        }

        private void UpdateOptions()
        {
            Options.Clear();

            // Lägg till alternativen i slumpmässig ordning
            foreach (var option in model.IncorrectAnswers.Append(model.CorrectAnswer).OrderBy(_ => Guid.NewGuid()))
            {
                Options.Add(option);
            }

            RaisePropertyChanged(nameof(Options));
        }
    }
}