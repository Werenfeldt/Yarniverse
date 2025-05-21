using MongoDB.Bson;

namespace Application.Model;

public record Yarn(string Name, Producer Producer, Gauge Gauge)
{
    public ObjectId Id { get; set; }
}