using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TinasLabb03.Model
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // MongoDB skapar unikt ID

        public string Name { get; set; }

        public Category(string name)
        {
            Name = name;
        }

        public Category()
        {
            Name = string.Empty;
        }
    }
}