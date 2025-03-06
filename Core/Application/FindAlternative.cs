using Database;
using Domain;
using MediatR;

namespace Application;

public record FindAlternativeSameNeedleOneThread(int RecipeGauge, double RecipeNeedle) : IRequest<List<Yarn>>;

public class FindAlternativeHandler(IMongoDb database) : IRequestHandler<FindAlternativeSameNeedleOneThread, List<Yarn>>
{
    public async Task<List<Yarn>> Handle(FindAlternativeSameNeedleOneThread request, CancellationToken cancellationToken)
    {
        var yarns = await database.GetByPredicateAsync<Yarn>(y => Math.Abs(y.Gauge.NeedleSize - request.RecipeNeedle) <= 0.5 && y.Gauge.Stitch == request.RecipeGauge);
        
        return yarns;
    }
}