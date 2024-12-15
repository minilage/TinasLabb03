using System.Collections.ObjectModel;
using TinasLabb03.Model;
using TinasLabb03.ViewModel;

public class QuestionViewModel : ViewModelBase
{
    private readonly Question model;
    private string? _selectedAnswer;

    public ObservableCollection<string> IncorrectAnswers { get; }
    public ObservableCollection<AnswerOptionViewModel> Options { get; }

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

        IncorrectAnswers.CollectionChanged += (sender, args) =>
        {
            model.IncorrectAnswers = IncorrectAnswers.ToArray();
            RaisePropertyChanged(nameof(IncorrectAnswers));
            UpdateOptions();
        };
    }

    private void UpdateOptions()
    {
        Options.Clear();

        // Lägg till felaktiga svar som AnswerOptionViewModel
        foreach (var incorrectAnswer in model.IncorrectAnswers)
        {
            Options.Add(new AnswerOptionViewModel(incorrectAnswer, false));
        }

        // Lägg till det korrekta svaret som AnswerOptionViewModel
        Options.Add(new AnswerOptionViewModel(model.CorrectAnswer, true));

        // Blanda alternativen
        var shuffledOptions = Options.OrderBy(_ => Guid.NewGuid()).ToList();
        Options.Clear();
        foreach (var option in shuffledOptions)
        {
            Options.Add(option);
        }

        RaisePropertyChanged(nameof(Options));
    }
}
