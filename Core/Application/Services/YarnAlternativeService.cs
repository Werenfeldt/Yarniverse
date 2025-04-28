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
        var needlePred = RangeHelper.NeedlesInRange(request.RecipeNeedle);
        var stitchPred = RangeHelper.StitchInRange(request.RecipeGauge);
        var pred = needlePred.AndAlso(stitchPred);
        
        var yarns = await database.GetByPredicateAsync<Yarn>(pred);
        
        var success = yarns.Any();
        
        return new Result(success){Message = success ? $"{yarns.Count} found" : "no yarn matches", Data = yarns};
    }
}