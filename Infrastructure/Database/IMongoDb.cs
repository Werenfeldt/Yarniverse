namespace Database;

public interface IMongoDb
{
    public Task<Yarn> InsertElement();
}