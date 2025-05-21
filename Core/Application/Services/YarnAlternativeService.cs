using Application.AlgorithmStrategies;
using Application.Helpers;
using Application.Model;
using Database;

namespace Application.Services;

public class YarnAlternativeService(IMongoDb database) : IYarnAlternativeService
{
    public async Task<Result> GetYarnAlternatives(FindAlternativeSameNeedleOneThread request, CancellationToken cancellationToken)
    {
        var yarns = await FindSuggestions(request.RecipeGauge, request.RecipeNeedle);
        
        var success = yarns.Any();
        
        return new Result(success){Message = success ? $"{yarns.Count} found" : "no yarn matches", Data = yarns};
    }
    
    private async Task<List<YarnSuggestion>> FindSuggestions(double targetGauge, double targetNeedle, string? preferredFiber = null)
    {
        var suggestions = await FindSingleYarnSuggestions(targetGauge, targetNeedle);

        return suggestions.OrderBy(s => s.Score).Take(10).ToList();
    }

    public async Task<IEnumerable<YarnSuggestion>> FindSingleYarnSuggestions(double targetGauge, double targetNeedle)
    {
        const double gaugeTolerance = 2.0;
        
        var pred = YarnMath.SingleYarn(targetGauge, targetNeedle, gaugeTolerance);

        var yarns = await database.GetByPredicateAsync(pred);

        return yarns.Select(x =>
            new YarnSuggestion(x)
            {
                EstimatedGauge = YarnMath.AdjustGaugeToNeedle(x,targetNeedle),
                TargetNeedle = targetNeedle,
                Score = CalculateScore(YarnMath.AdjustGaugeToNeedle(x,targetNeedle), targetGauge)
            }
        );
    }
    
    private double CalculateScore(double estimatedGauge, double targetGauge)
    {
        //double gaugeDiff = 1.0 - Math.Abs(estimatedGauge - targetGauge) / targetGauge;
        double gaugeDiff = Math.Abs(estimatedGauge - targetGauge);
        //double pricePenalty = price / 10.0; // e.g., 50 kr yarn = +5 points
        //double fiberPenalty = preferredFiber == null || fiberCombo.Contains(preferredFiber, StringComparison.OrdinalIgnoreCase) ? 0 : 5;

        return gaugeDiff * 2 /*+ pricePenalty + fiberPenalty*/;
    }
}