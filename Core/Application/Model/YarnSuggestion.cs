namespace Application.Model;

public class YarnSuggestion(string type, Yarn yarn1)
{
        public string Type { get; init; } = type; // "single" or "combo"
        public Yarn Yarn1 { get; init; } = yarn1;
        public Yarn? Yarn2 { get; set; } // null for single yarn
        public double EstimatedGauge { get; set; }
        
        public double TargetNeedle { get; set; }
        public double Score { get; set; } // lower = better
}