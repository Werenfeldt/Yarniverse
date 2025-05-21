using MongoDB.Bson;

namespace Application.Model;

public record Producer(string Name)
{
    public ObjectId Id { get; init; } = ObjectId.GenerateNewId();
}