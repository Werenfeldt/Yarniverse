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

        var success = yarns.Any();

        return new Result(success)
            { Message = success ? $"{yarns.ToList().Count} found" : "no yarn matches", Data = yarns };
    }

    public async Task<IEnumerable<YarnSuggestion>> FindSingleYarnSuggestions(double targetGauge, double targetNeedle)
    {
        const double gaugeTolerance = 1.0;

        var pred = YarnMath.SingleYarn(targetGauge, targetNeedle, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);

        return yarns.Select(x => EvaluateYarnMatch(x, targetGauge, targetNeedle))
            .OrderBy(result => result.Score)
            .ToList();
        ;
    }

    public YarnSuggestion EvaluateYarnMatch(Yarn yarn, double targetGauge, double targetNeedle,
        double gaugeTolerance = 1.0, double needleWeight = 0.5)
    {
        var evalYarn = new YarnSuggestion(yarn);
        
        evalYarn.SuggestedNeedleForTargetGauge = GetSuggestedNeedleForTargetGauge(yarn, targetGauge, targetNeedle);
        evalYarn.GaugeDifference = GetGaugeDifference(yarn, targetGauge, evalYarn.SuggestedNeedleForTargetGauge);
        evalYarn.Score = GetScore(yarn.Gauge.StitchAverage, targetGauge, yarn.Gauge.NeedleAverage, targetNeedle, needleWeight);
        evalYarn.DensityTag = GetDensityTag(targetGauge, gaugeTolerance);
        
        return evalYarn;
    }

    private double GetScore(double gaugeCenter, double targetGauge, double needleCenter, double targetNeedle,
        double needleWeight)
    {
        return Math.Abs(gaugeCenter - targetGauge) + Math.Abs(needleCenter - targetNeedle) * needleWeight;
    }

    private DensityTag GetDensityTag(double gaugeDifference, double gaugeTolerance )
    {
        if (gaugeDifference > gaugeTolerance)
            return DensityTag.Loose; // Yarn gives fewer stitches per 10cm → more open fabric

        if (gaugeDifference < -gaugeTolerance)
            return DensityTag.Dense; // Yarn gives more stitches per 10cm → tighter fabric
        
        return DensityTag.Neutral;
    }

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

    private double GetGaugeDifference(Yarn yarn, double targetGauge, double suggestedNeedleForTargetGauge)
    {
        var sMin = yarn.Gauge.StitchRange.Min;       
        var sMax = yarn.Gauge.StitchRange.Max;       
        var nMin = yarn.Gauge.NeedleRange.Min;       
        var nMax = yarn.Gauge.NeedleRange.Max;       
        
        // Calculate actual achievable gauge at suggested needle (inverse interpolation)     
        if (Math.Abs(nMax - nMin) <= 0.0)
        {
            return Math.Round(yarn.Gauge.StitchAverage - targetGauge, 2);
        }                                                                                                                
        else                                                                                                             
        {                                                                                                                
            var achievedGauge = sMax - (sMax - sMin) * (suggestedNeedleForTargetGauge - nMin) / (nMax - nMin);  
            return Math.Round(achievedGauge - targetGauge, 2); 
        }                                                                                                                
    }
}