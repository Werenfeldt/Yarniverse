using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Database;

public class MongoDb : IMongoDb
{
    private readonly IMongoDatabase _database;
    public MongoDb(IConfiguration configuration)
    {
        var hej = configuration.GetConnectionString("Mongo_Db");
        var client = new MongoClient(configuration.GetConnectionString("Mongo_Db"));
        _database = client.GetDatabase("Yarniverse");
    }
    public async Task<Yarn> InsertElement()
    {
        var collection = _database.GetCollection<Yarn>("Yarn");

        var yarn = new Yarn(
            ObjectId.GenerateNewId(),
            "Pernilla",
            new Producer(ObjectId.GenerateNewId(),
                "Filcolana"),
            "Green");
        
        await collection.InsertOneAsync(yarn);
        
        return yarn;
    }
}

public record Yarn(ObjectId Id, string Name, Producer Producer, string Color);

public record Producer(ObjectId Id, string Name);
