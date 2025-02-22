using Domain;
using MongoDB.Driver;

namespace Database;

public interface IMongoDb
{
    public Task<bool> InsertElements(List<Yarn> yarns);
    public Task<DeleteResult> DeleteElement(string id);
}