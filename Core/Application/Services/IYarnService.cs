using Application.Commands;
using Application.Model;

namespace Application.Services;

public interface IYarnService
{
    public Task<Result> CreateYarn(CreateYarnCommand request, CancellationToken cancellationToken = default);

    public Task<Result> DeleteYarn(string id, CancellationToken cancellationToken = default);
}