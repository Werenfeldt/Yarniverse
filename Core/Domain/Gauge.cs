using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record Gauge(int Stitch, double NeedleSize)
{
    public Guid Id { get; set; } = Guid.NewGuid();     
}