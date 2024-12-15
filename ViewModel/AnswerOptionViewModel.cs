using TinasLabb03.ViewModel;

public class AnswerOptionViewModel : ViewModelBase
{
    private bool _isSelected;
    private bool _isCorrect;

    public string Text { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            RaisePropertyChanged();
        }
    }

    public bool IsCorrect
    {
        get => _isCorrect;
        set
        {
            _isCorrect = value;
            RaisePropertyChanged();
        }
    }

    public AnswerOptionViewModel(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
