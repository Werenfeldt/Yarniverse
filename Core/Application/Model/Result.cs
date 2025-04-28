namespace Application.Model;

public record Result(bool IsSuccess)
{
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
    public object? Data { get; init; }
}