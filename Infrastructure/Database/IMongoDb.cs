using MongoDB.Driver;

namespace Database;

public interface IMongoDb
{
    public Task<Yarn> InsertElement();
    public Task<DeleteResult> DeleteElement(string id);
}