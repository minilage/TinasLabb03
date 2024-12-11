using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace TinasLabb03.Model
{
    public class QuestionPack
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))] // Lägg till detta
        public Difficulty Difficulty { get; set; }

        public int TimeLimitInSeconds { get; set; }
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public QuestionPack()
        {
            Name = "Default Name";
        }

        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
        }

        public QuestionPack(string name, Difficulty difficulty, int timeLimitInSeconds, ObservableCollection<Question> questions)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = questions;
        }
    }
}
