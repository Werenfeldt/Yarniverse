using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Database;

public class MongoDb : IMongoDb
{
    private readonly IMongoDatabase _database;
    public MongoDb(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("Mongo_Db"));
        _database = client.GetDatabase("Yarniverse");
    }
    public async Task<bool> InsertElements<T>(List<T>? elements)
    {
        if (elements == null || elements.Count == 0) return false;
        
        var collection = GetCollection<T>();

        if (collection == null)
            throw new ArgumentException($"Unsupported yarn type: {typeof(T).Name}");

        try
        {
            await collection.InsertManyAsync(elements, new InsertManyOptions { IsOrdered = false });
        }
        catch (MongoBulkWriteException<T> ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
        
        return true;
    }

    public async Task<DeleteResult> DeleteElement<T>(string id)
    {
        var collection = GetCollection<T>();
        // Build a filter to locate the document by id
        var filter = Builders<T>.Filter.Eq("_id", Guid.Parse(id));
        return await collection.DeleteOneAsync(filter);
    }
    
    public async Task<List<T>> GetByPredicateAsync<T>(Expression<Func<T, bool>> predicate)
    {
        var collection = GetCollection<T>();
        
        return await collection.Find(predicate).ToListAsync();
    }

    private IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = typeof(T).Name + "s";
        var collectionSetup = !_database.ListCollectionNames().ToList().Contains(collectionName);
        var collection = _database.GetCollection<T>(collectionName);
        if (collectionSetup)
        {
            // Create index if the collection is empty (only during initial setup)
            var nameProperty = typeof(T).GetProperty("Name");

            if (nameProperty != null)
            {
                var indexKeys = Builders<T>.IndexKeys.Ascending(nameProperty.Name);
                var indexOptions = new CreateIndexOptions { Unique = true };
                var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);

                // Create the unique index on the "Name" property
                collection.Indexes.CreateOne(indexModel);
            }
        }
        return collection;
    }
}
