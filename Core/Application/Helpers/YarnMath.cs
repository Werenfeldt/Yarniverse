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
    
    public static double AdjustGaugeToNeedle(Yarn yarn1, Yarn yarn2, double newNeedle)
    {
        // Basic linear approximation
        return EstimateCombinedGauge(yarn1.Gauge.Stitch, yarn2.Gauge.Stitch) * (EstimateCombinedNeedle(yarn1.Gauge.NeedleSize, yarn2.Gauge.NeedleSize) / newNeedle);
    }

    private static double EstimateCombinedNeedle(Range<double> needle1, Range<double> needle2)
    {
        return (Average(needle1) + Average(needle2)) / 2;
    }
    private static double EstimateCombinedGauge(Range<int> gauge1, Range<int> gauge2)
    {
        // Harmonic mean approximation for held-together yarns
        return 1 / (1 / Average(gauge1) + 1 / Average(gauge2));
    }
    
    // TODO change such that the Average doesnt need to be written out. 
    public static Expression<Func<Yarn, bool>> SingleYarn(double targetGauge, double targetNeedle, double gaugeTolerance) => yarn =>
        Math.Abs(
            Math.Round((yarn.Gauge.Stitch.Min+yarn.Gauge.Stitch.Max)/2.0, 1) * 
            (Math.Round((yarn.Gauge.NeedleSize.Min+yarn.Gauge.NeedleSize.Max)/2.0, 1) / targetNeedle) - targetGauge) <= gaugeTolerance;
    
    //public static Expression<Func<Yarn>>
    
    private static double Average(Range<int> gauge) => Math.Round((gauge.Min+gauge.Max)/2.0, 1);
    private static double Average(Range<double> gauge) => Math.Round((gauge.Min+gauge.Max)/2.0, 1);
    
    public static Expression<Func<Yarn, double>> GaugeAverage() => yarn => Math.Round((yarn.Gauge.Stitch.Min + yarn.Gauge.Stitch.Max) / 2.0, 1);
    
    public static Expression<Func<Yarn, double>> NeedleAverage(double targetNeedle) => yarn => Math.Round((yarn.Gauge.Stitch.Min + yarn.Gauge.Stitch.Max) / 2.0, 1) / targetNeedle;
}