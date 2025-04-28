namespace Application.Model;

public record Producer(string Name)
{
    public Guid Id { get; set; } = Guid.NewGuid();     
}