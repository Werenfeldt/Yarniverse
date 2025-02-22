using Domain;
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
    public async Task<bool> InsertElements(List<Yarn> yarns)
    {
        var collection = _database.GetCollection<Yarn>("Yarn");
        try
        {
            await collection.InsertManyAsync(yarns);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<DeleteResult> DeleteElement(string id)
    {
        var collection = _database.GetCollection<Yarn>("Yarn");
        // Build a filter to locate the document by id
        return await collection.DeleteOneAsync(x => x.Id == Guid.Parse(id));
    }
}
