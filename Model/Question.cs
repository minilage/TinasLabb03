using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TinasLabb03.Model
{
    public class Question : INotifyPropertyChanged
    {
        private string _query;
        private string _correctAnswer;
        private string[] _incorrectAnswers;
        private Difficulty _difficulty; // Lägg till en fältvariabel för Difficulty

        [JsonConstructor]
        public Question(string query, string correctAnswer, string[] incorrectAnswers, Difficulty difficulty)
        {
            _query = query;
            _correctAnswer = correctAnswer;
            _incorrectAnswers = incorrectAnswers;
            _difficulty = difficulty; // Sätt värdet för Difficulty
        }

        public Question()
        {
            _query = string.Empty;
            _correctAnswer = string.Empty;
            _incorrectAnswers = new[] { string.Empty, string.Empty, string.Empty };
            _difficulty = Difficulty.Medium; // Standardvärde
        }

        [JsonPropertyName("question")]
        public string Query
        {
            get => _query;
            set
            {
                if (_query != value)
                {
                    _query = value;
                    OnPropertyChanged(nameof(Query));
                }
            }
        }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer
        {
            get => _correctAnswer;
            set
            {
                if (_correctAnswer != value)
                {
                    _correctAnswer = value;
                    OnPropertyChanged(nameof(CorrectAnswer));
                }
            }
        }

        [JsonPropertyName("incorrect_answers")]
        public string[] IncorrectAnswers
        {
            get => _incorrectAnswers;
            set
            {
                if (_incorrectAnswers != value)
                {
                    _incorrectAnswers = value;
                    OnPropertyChanged(nameof(IncorrectAnswers));
                }
            }
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Difficulty Difficulty
        {
            get => _difficulty;
            set
            {
                if (_difficulty != value)
                {
                    _difficulty = value;
                    OnPropertyChanged(nameof(Difficulty));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
