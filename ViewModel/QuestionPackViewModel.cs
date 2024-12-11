using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    public class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;

        public QuestionPackViewModel(QuestionPack model)
        {
            this.model = model;
            Questions = new ObservableCollection<Question>(model.Questions);
        }

        public string Name
        {
            get => model.Name;
            set
            {
                model.Name = value;
                RaisePropertyChanged();
            }
        }

        public Difficulty Difficulty
        {
            get => model.Difficulty;
            set
            {
                model.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        public int TimeLimitInSeconds
        {
            get => model.TimeLimitInSeconds;
            set
            {
                model.TimeLimitInSeconds = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Question> Questions { get; }

        [JsonConstructor]
        public QuestionPackViewModel(string name, Difficulty difficulty, int timeLimitInSeconds, ObservableCollection<Question> questions)
        {
            model = new QuestionPack();
            Questions = questions;
            model.Name = name;
            model.Difficulty = difficulty;
            model.TimeLimitInSeconds = timeLimitInSeconds;
        }
    }
}
