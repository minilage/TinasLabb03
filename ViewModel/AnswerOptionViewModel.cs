namespace TinasLabb03.ViewModel
{
    public class AnswerOptionViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isCorrect;
        private bool _isWrong;

        public string Text { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsCorrect
        {
            get => _isCorrect;
            set
            {
                if (_isCorrect != value)
                {
                    _isCorrect = value;
                    RaisePropertyChanged(nameof(IsCorrect));
                }
            }
        }

        public bool IsWrong
        {
            get => _isWrong;
            set
            {
                if (_isWrong != value)
                {
                    _isWrong = value;
                    RaisePropertyChanged(nameof(IsWrong));
                }
            }
        }

        public AnswerOptionViewModel(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }

        // Method to reset the state of the answer option
        public void Reset()
        {
            IsSelected = false;
            IsWrong = false;
        }
    }
}