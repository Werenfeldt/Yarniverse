using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record Producer(string Name)
{
    public Guid Id { get; set; } = Guid.NewGuid();     
}