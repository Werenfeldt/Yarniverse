using System.Linq.Expressions;
using Application.Model;

namespace Application.Helpers;

public static class YarnMath
{
    public static Expression<Func<Yarn, bool>> SingleYarn(double targetGauge, double gaugeTolerance) => yarn =>
        targetGauge >= yarn.Gauge.StitchRange.Min - gaugeTolerance &&
        targetGauge <= yarn.Gauge.StitchRange.Max + gaugeTolerance;
}