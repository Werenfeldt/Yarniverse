using System.Text.Json.Serialization;

namespace Application.Model;

public record YarnSuggestion(Yarn Yarn)
{
    public double Score { get; set; } // lower = better
        
    public DensityTag DensityTag { get; set; }
        
    public double SuggestedNeedleForTargetGauge { get; set; }
        
    public double GaugeDifference { get; set; } // Positive = looser, negative = denser
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DensityTag
{
    Neutral,
    Dense,
    Loose
}