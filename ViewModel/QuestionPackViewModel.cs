using System.Collections.ObjectModel;
using TinasLabb03.Model;

namespace TinasLabb03.ViewModel
{
    // ViewModel för ett QuestionPack, exponerar egenskaper som bindas i UI:t.
    public class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;

        public QuestionPackViewModel(QuestionPack model)
        {
            this.model = model;
            // Konverterar modellens frågor till QuestionViewModels
            Questions = new ObservableCollection<QuestionViewModel>(
                model.Questions.Select(q => new QuestionViewModel(q))
            );
        }

        // Exponera modellens Id
        public string? Id => model.Id;

        // Packets namn med TwoWay-binding
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

        // Exponerar kategori så att UI:t kan binda mot den
        public string Category
        {
            get => model.Category;
            set
            {
                model.Category = value;
                RaisePropertyChanged();
            }
        }


        // Lista med frågor (ViewModels) i paketet.
        public ObservableCollection<QuestionViewModel> Questions { get; }

        // Konverterar detta QuestionPackViewModel till Modelobjektet.
        //public QuestionPack ToModel() => model;

        public QuestionPack ToModel()
        {
            // Kopiera grundläggande egenskaper
            model.Name = this.Name;
            model.Difficulty = this.Difficulty;
            model.TimeLimitInSeconds = this.TimeLimitInSeconds;
            model.Category = this.Category;

            // Töm modellens Questions och fyll på med nya
            model.Questions.Clear();
            foreach (var qvm in this.Questions)
            {
                model.Questions.Add(qvm.ToModel());
            }

            return model;
        }

    }
}
