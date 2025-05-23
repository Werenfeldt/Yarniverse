using Application.AlgorithmStrategies;
using Application.Model;

namespace Application.Services;

public interface IYarnAlternativeService
{
    Task<Result> GetYarnAlternatives(FindAlternativeSameNeedleOneThread request, CancellationToken cancellationToken = default);

    Task<IEnumerable<YarnSuggestion>> FindSingleYarnSuggestions(double targetGauge, double targetNeedle);
}