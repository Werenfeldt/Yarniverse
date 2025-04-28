using Application.Helpers;

namespace Application.Model;

public record Gauge(Range<int> Stitch, Range<double> NeedleSize)
{
    public Guid Id { get; set; } = Guid.NewGuid();     
}