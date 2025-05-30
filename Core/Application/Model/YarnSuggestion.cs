using System.Text.Json.Serialization;

namespace Application.Model;

public record YarnSuggestion(Yarn Yarn)
{
    public double Score { get; init; } // lower = better
        
    public DensityTag DensityTag { get; init; }
        
    public double SuggestedNeedleForTargetGauge { get; init; }
        
    public double GaugeDifference { get; init; } // Positive = looser, negative = denser
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DensityTag
{
    Neutral,
    Dense,
    Loose
}