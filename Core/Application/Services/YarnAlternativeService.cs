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

    private YarnSuggestion EvaluateYarnMatch(Yarn yarn, int targetGauge, double targetNeedle, double needleWeight = 0.5)
    {
        Console.WriteLine($"Evaluating yarn: {yarn.Name}");
        var (suggestedNeedle , gaugeDiff) = GetClosestNeedleMatchByGauge(yarn, targetGauge);
        //var gaugeDifference = GetGaugeDifference(yarn, targetGauge, suggestedNeedle);
        
        
        return new YarnSuggestion(yarn)
        {
            SuggestedNeedleForTargetGauge = suggestedNeedle,
            GaugeDifference = gaugeDiff,
            Score = GetScore(yarn, gaugeDiff, suggestedNeedle, targetNeedle, needleWeight),
            DensityTag = GetDensityTag(gaugeDiff)
        };
    }

    private double GetScore(Yarn yarn, double gaugeDiff, double suggestedNeedle, double targetNeedle,
        double needleWeight)
    {
        
        if (IsPerfectMatch(yarn, gaugeDiff, suggestedNeedle)) return 0;
        
        return Math.Abs(gaugeDiff) + Math.Abs(suggestedNeedle - targetNeedle) * needleWeight;
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
                    GaugeDiffAbs = Math.Abs(gauge - targetGauge),
                    GaugeDiff = targetGauge - gauge
                };
            })
            .Where(x => x.GaugeDiffAbs <= 2)
            .OrderBy(x => x.GaugeDiffAbs)
            .ThenBy(x => x.Needle)
            .First();
        
        switch (mappedList.GaugeDiff)
        {
            case >= 2:
                return (mappedList.Needle + 0.5, mappedList.GaugeDiff);
            case <= -2:
                return (mappedList.Needle - 0.5, mappedList.GaugeDiff);
            default:
                ;
                return (mappedList.Needle, mappedList.GaugeDiff);
        }
    }
    
    private bool IsPerfectMatch(Yarn yarn, double gaugeDiff, double suggestedNeedle)
    {
        return
            gaugeDiff.Equals(0) && (yarn.Gauge.NeedleRange.Min.Equals(suggestedNeedle) ||
            yarn.Gauge.NeedleRange.Max.Equals(suggestedNeedle) ||
            yarn.Gauge.NeedleAverage.Equals(suggestedNeedle));
    }
}