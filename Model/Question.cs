using System.Text.Json.Serialization;
using TinasLabb03.ViewModel;

namespace TinasLabb03.Model
{
    public class Question
    {
        [JsonPropertyName("question")]
        public string Query { get; set; }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("incorrect_answers")]
        public string[] IncorrectAnswers { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Difficulty Difficulty { get; set; }

        public Question(string query, string correctAnswer, string[] incorrectAnswers, Difficulty difficulty)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = incorrectAnswers;
            Difficulty = difficulty;
        }

        public Question()
        {
            Query = string.Empty;
            CorrectAnswer = string.Empty;
            IncorrectAnswers = new[] { string.Empty, string.Empty, string.Empty };
            Difficulty = Difficulty.Medium;
        }

        public static implicit operator Question?(QuestionViewModel? v)
        {
            throw new NotImplementedException();
        }
    }
}
