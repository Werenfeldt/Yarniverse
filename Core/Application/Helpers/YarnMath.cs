using System.Linq.Expressions;
using Application.Model;

namespace Application.Helpers;

public static class YarnMath
{
    public static double AdjustGaugeToNeedle(Yarn yarn, double targetNeedle)
    {
        // Basic linear approximation
        return Math.Round(yarn.Gauge.StitchAverage * yarn.Gauge.NeedleAverage / targetNeedle,1);
    }
    
    // TODO change such that the Average doesnt need to be written out. 
    public static Expression<Func<Yarn, bool>> SingleYarn(double targetGauge, double targetNeedle, double gaugeTolerance) => yarn =>
        Math.Abs( 
            Math.Round((yarn.Gauge.StitchRange.Min+yarn.Gauge.StitchRange.Max)/2.0, 1) * 
            (Math.Round((yarn.Gauge.NeedleRange.Min+yarn.Gauge.NeedleRange.Max)/2.0, 1) / targetNeedle) - targetGauge) <= gaugeTolerance;
}