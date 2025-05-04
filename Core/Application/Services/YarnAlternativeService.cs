using Application.AlgorithmStrategies;
using Application.Helpers;
using Application.Model;
using RangeHelper = Application.Helpers.RangeHelper;
using Database;

namespace Application.Services;

public class YarnAlternativeService(IMongoDb database) : IYarnAlternativeService
{
    public async Task<Result> GetYarnAlternatives(FindAlternativeSameNeedleOneThread request, CancellationToken cancellationToken)
    {
        // var needlePred = RangeHelper.NeedlesInRange(request.RecipeNeedle);
        // var stitchPred = RangeHelper.StitchInRange(request.RecipeGauge);
        // var pred = needlePred.AndAlso(stitchPred);
        //
        // var yarns = await database.GetByPredicateAsync(pred);

        var yarns = await FindSuggestions(request.RecipeGauge, request.RecipeNeedle);
        
        var success = yarns.Any();
        
        return new Result(success){Message = success ? $"{yarns.Count} found" : "no yarn matches", Data = yarns};
    }
    
    private async Task<List<YarnSuggestion>> FindSuggestions(double targetGauge, double targetNeedle, string? preferredFiber = null)
    {
        var suggestions = new List<YarnSuggestion>();
        suggestions.AddRange(await FindSingleYarnSuggestions(targetGauge, targetNeedle));
        
        suggestions.AddRange(await FindDoubleYarnSuggestions(targetGauge, targetNeedle));

        return suggestions.OrderBy(s => s.Score).Take(10).ToList();
    }

    private async Task<IEnumerable<YarnSuggestion>> FindSingleYarnSuggestions(double targetGauge, double targetNeedle)
    {
        const double gaugeTolerance = 1.0;
        
        var pred = YarnMath.SingleYarn(targetGauge, targetNeedle, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);

        return yarns.Select(x =>
            new YarnSuggestion("single", x)
            {
                EstimatedGauge = YarnMath.AdjustGaugeToNeedle(x,targetNeedle),
                TargetNeedle = targetNeedle,
                Score = CalculateScore(YarnMath.AdjustGaugeToNeedle(x,targetNeedle), targetGauge)
            }
        );
    }
    
    private async Task<IEnumerable<YarnSuggestion>> FindDoubleYarnSuggestions(double targetGauge, double targetNeedle)
    {
        const double gaugeTolerance = 3.0;

        var pred = YarnMath.SingleYarn(targetGauge, targetNeedle, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);
        var result = new List<YarnSuggestion>();
        foreach (var yarn1 in yarns)
        {
            foreach (var yarn2 in yarns)
            {
                if (yarn1.Name == yarn2.Name) continue; // skip same yarn twice unless you allow it

                double adjusted = YarnMath.AdjustGaugeToNeedle(yarn1, yarn2, targetNeedle);

                if (Math.Abs(adjusted - targetGauge) <= gaugeTolerance)
                {
                    result.Add(new YarnSuggestion("combo", yarn1)
                    {
                        Yarn2 = yarn2,
                        EstimatedGauge = adjusted,
                        Score = CalculateScore(adjusted, targetGauge)
                    });
                }
            }
        }
        return result;
    }

    private double CalculateScore(double estimatedGauge, double targetGauge)
    {
        double gaugeDiff = Math.Abs(estimatedGauge - targetGauge);
        //double pricePenalty = price / 10.0; // e.g., 50 kr yarn = +5 points
        //double fiberPenalty = preferredFiber == null || fiberCombo.Contains(preferredFiber, StringComparison.OrdinalIgnoreCase) ? 0 : 5;

        return gaugeDiff * 2 /*+ pricePenalty + fiberPenalty*/;
    }
}