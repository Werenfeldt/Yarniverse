using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record Yarn(string Name, Producer Producer, string Color, Gauge Gauge)
{
    public Guid Id { get; set; } = Guid.NewGuid();
}