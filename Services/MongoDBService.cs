using MongoDB.Driver;
using System.Collections.ObjectModel;
using TinasLabb03.ViewModel;

internal class MongoDBService
{

    static string connectionString = "mongodb://localhost:27017";

    public void Save(ObservableCollection<QuestionPackViewModel> packs)
    {
        MongoClient client = new(connectionString);
        var collection = client.GetDatabase("TinaLagesson").GetCollection<QuestionPackViewModel>("QuestionPack");

        foreach (var pack in packs)
        {
            collection.InsertOne(pack);
        }
    }

    //public class TinasLabb03
    //{
    //    private readonly IMongoDatabase? _database;
    //    public IMongoCollection<Category>? Categories { get; set; }
    //    public IMongoCollection<QuestionPack>? QuestionPacks { get; set; }
    //}
}