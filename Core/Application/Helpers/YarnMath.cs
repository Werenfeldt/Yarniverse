using System.Linq.Expressions;
using Application.Model;

namespace Application.Helpers;

public static class YarnMath
{
    public static double AdjustGaugeToNeedle(Yarn yarn, double targetNeedle)
    {
        // Basic linear approximation
        return Math.Round(Average(yarn.Gauge.Stitch) * (Average(yarn.Gauge.NeedleSize) / targetNeedle),1);
    }

    public static double EstimateCombinedGauge(Yarn yarn1, Yarn yarn2, double targetNeedle)
    {
        var thickness1 = EstimateThickness(Average(yarn1.Gauge.Stitch));
        var thickness2 = EstimateThickness(Average(yarn2.Gauge.Stitch));
        // Combine yarn thicknesses geometrically
        double combinedThickness = Math.Sqrt(thickness1 * thickness1 + thickness2 * thickness2);

        // ✨ Calibration scaling factor
        combinedThickness *= 0.82;
        
        // Adjust for the effect of needle size
        double avgRecommendedNeedle = (Average(yarn1.Gauge.NeedleSize) + Average(yarn2.Gauge.NeedleSize)) / 2.0;
        // Empirical: needle change has more than linear effect
        double needleRatio = targetNeedle / avgRecommendedNeedle;
        
        // Dynamic exponent scaling
        double exponent = 0.5 + 0.3 * Math.Min(1.0, Math.Abs(needleRatio - 1.0));
        double needleAdjustment = Math.Pow(needleRatio, exponent);

        double adjustedThickness = combinedThickness * needleAdjustment;

        // Return back to gauge
        return Math.Round(10.0 / adjustedThickness, 1);
    }

    public static double EstimateThickness(double gauge)
    {
        //if (gauge <= 0 || needle <= 0)
          //  throw new ArgumentException("Gauge and needle must be positive");
        return 10.0 / gauge;
    }
    
    // TODO change such that the Average doesnt need to be written out. 
    public static Expression<Func<Yarn, bool>> SingleYarn(double targetGauge, double targetNeedle, double gaugeTolerance) => yarn =>
        Math.Abs(
            Math.Round((yarn.Gauge.Stitch.Min+yarn.Gauge.Stitch.Max)/2.0, 1) * 
            (Math.Round((yarn.Gauge.NeedleSize.Min+yarn.Gauge.NeedleSize.Max)/2.0, 1) / targetNeedle) - targetGauge) <= gaugeTolerance;
    
    private static double Average(Range<int> gauge) => Math.Round((gauge.Min+gauge.Max)/2.0, 1);
    private static double Average(Range<double> gauge) => Math.Round((gauge.Min+gauge.Max)/2.0, 1);
    
    public static Expression<Func<Yarn, double>> GaugeAverage() => yarn => Math.Round((yarn.Gauge.Stitch.Min + yarn.Gauge.Stitch.Max) / 2.0, 1);
    
    public static Expression<Func<Yarn, double>> NeedleAverage(double targetNeedle) => yarn => Math.Round((yarn.Gauge.Stitch.Min + yarn.Gauge.Stitch.Max) / 2.0, 1) / targetNeedle;
}