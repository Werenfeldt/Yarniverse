namespace Application.Model;

public record YarnSuggestion(Yarn Yarn)
{
    public double Score { get; set; } // lower = better
        
    public DensityTag DensityTag { get; set; }
        
    public double SuggestedNeedleForTargetGauge { get; set; }
        
    public double GaugeDifference { get; set; } // Positive = looser, negative = denser
}

public enum DensityTag
{
    Neutral,
    Dense,
    Loose
}