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
        const double gaugeTolerance = 1.0;

        var pred = YarnMath.SingleYarn(targetGauge, targetNeedle, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);

        return yarns.Select(x => EvaluateYarnMatch(x, targetGauge, targetNeedle))
            .OrderBy(result => result.Score)
            .ToList();
    }

    private YarnSuggestion EvaluateYarnMatch(Yarn yarn, int targetGauge, double targetNeedle,
        double gaugeTolerance = 1.0, double needleWeight = 0.5)
    {
        var suggestedNeedle = GetSuggestedNeedleForTargetGauge(yarn, targetGauge, targetNeedle);
        var gaugeDifference = GetGaugeDifference(yarn, targetGauge, suggestedNeedle);
        
        return new YarnSuggestion(yarn)
        {
            SuggestedNeedleForTargetGauge = suggestedNeedle,
            GaugeDifference = gaugeDifference,
            Score = GetScore(yarn, yarn.Gauge.StitchAverage, targetGauge, yarn.Gauge.NeedleAverage, targetNeedle, needleWeight),
            DensityTag = GetDensityTag(gaugeDifference, gaugeTolerance)
        };
    }

    private double GetScore(Yarn yarn, int gaugeCenter, int targetGauge, double needleCenter, double targetNeedle,
        double needleWeight)
    {
        
        if (IsPerfectMatch(yarn, targetGauge, gaugeCenter, targetNeedle, needleCenter)) return 0;
        
        return Math.Abs(gaugeCenter - targetGauge) + Math.Abs(needleCenter - targetNeedle) * needleWeight;
    }

    private DensityTag GetDensityTag(double gaugeDifference, double gaugeTolerance )
    {
        if (gaugeDifference > gaugeTolerance)
            return DensityTag.Loose; // Yarn gives fewer stitches per 10 cm → more open fabric

        if (gaugeDifference < -gaugeTolerance)
            return DensityTag.Dense; // Yarn gives more stitches per 10 cm → tighter fabric
        
        return DensityTag.Neutral;
    }

    // TODO if needlerange is right ouside suggest best needle outside range. 
private double GetSuggestedNeedleForTargetGauge(Yarn yarn, double targetGauge, double targetNeedle)
    {
        var nMax = yarn.Gauge.NeedleRange.Max; 
        
        return Enumerable
            .Range(0, (int)((yarn.Gauge.NeedleRange.Max - yarn.Gauge.NeedleRange.Min) / 0.5) + 1)
            .Select(i => Math.Round(yarn.Gauge.NeedleRange.Min + i * 0.5, 1))
            .TakeWhile(n => n <= nMax)
            .Select(n => new
            {
                Needle = Math.Round(n, 1),
                ExpectedGauge = yarn.Gauge.StitchAverage * (Math.Round(n, 1) / targetNeedle)
            })
            .Select(x => new {
                x.Needle,
                GaugeDifference = Math.Abs(x.ExpectedGauge - targetGauge)
            })
            .OrderBy(x => x.GaugeDifference)
            .First()
            .Needle;
    }

    private double GetGaugeDifference(Yarn yarn, int targetGauge, double suggestedNeedleForTargetGauge)
    {
        var sMin = yarn.Gauge.StitchRange.Min;       
        var sMax = yarn.Gauge.StitchRange.Max;       
        var nMin = yarn.Gauge.NeedleRange.Min;       
        var nMax = yarn.Gauge.NeedleRange.Max;

        if (IsPerfectMatch(yarn, targetGauge, yarn.Gauge.StitchAverage, suggestedNeedleForTargetGauge, yarn.Gauge.NeedleAverage)) return 0;
        
        // Calculate actual achievable gauge at suggested needle (inverse interpolation)     
        if (Math.Abs(nMax - nMin) <= 0.0)
        {
            return yarn.Gauge.StitchAverage - targetGauge;
        }                                                                                                                
                                                                                                              
        var achievedGauge = sMax - (sMax - sMin) * (suggestedNeedleForTargetGauge - nMin) / (nMax - nMin);  
        return Math.Round(achievedGauge - targetGauge, 2); 
                                                                                                                     
    }

    private bool IsPerfectMatch(Yarn yarn, int targetGauge, int gaugeCenter, double targetNeedle, double needleCenter)
    {
        return
            (yarn.Gauge.StitchRange.Min.Equals(targetGauge) && yarn.Gauge.NeedleRange.Min.Equals(targetNeedle)) ||
            (yarn.Gauge.StitchRange.Max.Equals(targetGauge) && yarn.Gauge.NeedleRange.Max.Equals(targetNeedle)) ||
            (gaugeCenter.Equals(targetGauge) && needleCenter.Equals(targetNeedle));
    }
}