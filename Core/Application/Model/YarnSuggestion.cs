namespace Application.Model;

public record YarnSuggestion(Yarn Yarn)
{
        public double EstimatedGauge { get; set; }
        
        public double TargetNeedle { get; set; }
        public double Score { get; set; } // lower = better
}