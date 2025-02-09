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
            "Green",
            new Gauge(ObjectId.GenerateNewId(),
                23, 
                4.5));
        
        await collection.InsertOneAsync(yarn);
        
        return yarn;
    }

    public async Task<DeleteResult> DeleteElement(string id)
    {
        var collection = _database.GetCollection<Yarn>("Yarn");
        // Build a filter to locate the document by id
        return await collection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));
    }
}

public record Yarn(ObjectId Id, string Name, Producer Producer, string Color, Gauge Gauge);

public record Producer(ObjectId Id, string Name);
public record Gauge(ObjectId Id, int gauge, double needleSize);
