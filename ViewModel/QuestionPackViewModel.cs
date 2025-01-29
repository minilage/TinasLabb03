using System.Collections.ObjectModel;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;

        public ObservableCollection<QuestionViewModel> Questions { get; }

        public QuestionPackViewModel(QuestionPack model)
        {
            this.model = model;
            Questions = new ObservableCollection<QuestionViewModel>(
                model.Questions.Select(q => new QuestionViewModel(q))
            );
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
    }
}
