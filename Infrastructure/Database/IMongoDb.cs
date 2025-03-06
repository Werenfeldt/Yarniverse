using System.Linq.Expressions;
using MongoDB.Driver;

namespace Database;

public interface IMongoDb
{
    public Task<bool> InsertElements<T>(List<T> yarns);
    public Task<DeleteResult> DeleteElement<T>(string id);
    public Task<List<T>> GetByPredicateAsync<T>(Expression<Func<T, bool>> predicate);
}