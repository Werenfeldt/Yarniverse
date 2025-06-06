namespace Application.Model;

public record Gauge(StitchRange StitchRange, NeedleRange NeedleRange)
{
    public int StitchAverage => (int)Math.Round((StitchRange.Min + StitchRange.Max) / 2.0, MidpointRounding.AwayFromZero);
    
    public double NeedleAverage => Math.Round((NeedleRange.Min + NeedleRange.Max) / 2.0, 1);
}

public record StitchRange(int Min, int Max);

public record NeedleRange(double Min, double Max);