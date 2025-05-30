using Application.AlgorithmStrategies;
using Application.Helpers;
using Application.Model;
using Database;

namespace Application.Services;

public class YarnAlternativeService(IMongoDb database) : IYarnAlternativeService
{
    public async Task<Result> GetYarnAlternatives(FindAlternativeSameNeedleOneThread request,
        CancellationToken cancellationToken)
    {
        var yarns = await FindSingleYarnSuggestions(request.RecipeGauge, request.RecipeNeedle);

        var yarnSuggestions = yarns.ToList();
        var success = yarnSuggestions.Any();

        return new Result(success)
            { Message = success ? $"{yarnSuggestions.Count} found" : "no yarn matches", Data = yarns };
    }

    public async Task<IEnumerable<YarnSuggestion>> FindSingleYarnSuggestions(int targetGauge, double targetNeedle)
    {
        const double gaugeTolerance = 2.0;

        var pred = YarnMath.SingleYarn(targetGauge, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);

        return yarns.Select(x => EvaluateYarnMatch(x, targetGauge, targetNeedle))
            .OrderBy(result => result.Score)
            .ToList();
    }

    private YarnSuggestion EvaluateYarnMatch(Yarn yarn, int targetGauge, double targetNeedle,
        double gaugeTolerance = 1.0, double needleWeight = 0.5)
    {
        var (suggestedNeedle , gaugeDiff) = GetClosestNeedleMatchByGauge(yarn, targetGauge);
        //var gaugeDifference = GetGaugeDifference(yarn, targetGauge, suggestedNeedle);
        
        return new YarnSuggestion(yarn)
        {
            SuggestedNeedleForTargetGauge = suggestedNeedle,
            GaugeDifference = gaugeDiff,
            Score = GetScore(yarn, yarn.Gauge.StitchAverage, targetGauge, yarn.Gauge.NeedleAverage, targetNeedle, needleWeight),
            DensityTag = GetDensityTag(gaugeDiff)
        };
    }

    private double GetScore(Yarn yarn, int gaugeCenter, int targetGauge, double needleCenter, double targetNeedle,
        double needleWeight)
    {
        
        if (IsPerfectMatch(yarn, targetGauge, gaugeCenter, targetNeedle, needleCenter)) return 0;
        
        return Math.Abs(gaugeCenter - targetGauge) + Math.Abs(needleCenter - targetNeedle) * needleWeight;
    }

    private DensityTag GetDensityTag(double gaugeDifference)
    {
        if (gaugeDifference < -0.25)
            return DensityTag.Loose; // Yarn gives fewer stitches per 10 cm → more open fabric

        if (gaugeDifference > 0.25)
            return DensityTag.Dense; // Yarn gives more stitches per 10 cm → tighter fabric
        
        return DensityTag.Neutral;
    }

    private (double, double) GetClosestNeedleMatchByGauge(Yarn yarn, double targetGauge)
    {
        double gaugeMin = yarn.Gauge.StitchRange.Min;
        double gaugeMax = yarn.Gauge.StitchRange.Max;
        double needleMin = yarn.Gauge.NeedleRange.Min;
        double needleMax = yarn.Gauge.NeedleRange.Max;

        int gaugeSteps = (int)(gaugeMax - gaugeMin);

        var mappedList = Enumerable
            .Range(0, gaugeSteps + 1)
            .Select(i =>
            {
                var gauge = gaugeMin + i;
                var ratio = (gauge - gaugeMin) / (gaugeMax - gaugeMin);
                var rawNeedle = needleMin + ratio * (needleMax - needleMin);
                var roundedNeedle = Math.Round(rawNeedle * 2, MidpointRounding.AwayFromZero) / 2.0;
                return new
                {
                    Gauge = gauge,
                    Needle = roundedNeedle,
                    GaugeDiff = Math.Abs(gauge - targetGauge)
                };
            })
            .Where(x => x.GaugeDiff <= 2)
            .OrderBy(x => x.GaugeDiff)
            .ThenBy(x => x.Needle)
            .First();

        // TODO look at this again 
        if (mappedList.Gauge <= targetGauge - 2) return (mappedList.Needle + 0.5, 2);;
        if (mappedList.Gauge >= targetGauge + 2) return (mappedList.Needle - 0.5, -2);;
        return (mappedList.Needle, 0);
    }

    private bool IsPerfectMatch(Yarn yarn, int targetGauge, int gaugeCenter, double targetNeedle, double needleCenter)
    {
        return
            (yarn.Gauge.StitchRange.Min.Equals(targetGauge) && yarn.Gauge.NeedleRange.Min.Equals(targetNeedle)) ||
            (yarn.Gauge.StitchRange.Max.Equals(targetGauge) && yarn.Gauge.NeedleRange.Max.Equals(targetNeedle)) ||
            (gaugeCenter.Equals(targetGauge) && needleCenter.Equals(targetNeedle));
    }
}