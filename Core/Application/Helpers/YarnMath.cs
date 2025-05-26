using System.Linq.Expressions;
using Application.Model;

namespace Application.Helpers;

public static class YarnMath
{
    public static Expression<Func<Yarn, bool>> SingleYarn(double targetGauge, double targetNeedle, double gaugeTolerance, double needleTolerance = 0.5) => yarn =>
        yarn.Gauge.StitchRange.Min <= targetGauge + gaugeTolerance &&
        yarn.Gauge.StitchRange.Max >= targetGauge - gaugeTolerance &&
        yarn.Gauge.NeedleRange.Min <= targetNeedle + needleTolerance &&
        yarn.Gauge.NeedleRange.Max >= targetNeedle - needleTolerance;
}