namespace Application.Model;

public record Yarn(string Name, Producer Producer, string Color, Gauge Gauge)
{
    public Guid Id { get; set; } = Guid.NewGuid();
}