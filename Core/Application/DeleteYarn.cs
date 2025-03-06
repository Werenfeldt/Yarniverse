using Database;
using Domain;
using MediatR;

namespace Application;

public record DeleteYarnCommand(string Id) : IRequest<bool>;

public class DeleteYarnHandler(IMongoDb database) : IRequestHandler<DeleteYarnCommand, bool>
{
    public async Task<bool> Handle(DeleteYarnCommand request, CancellationToken cancellationToken)
    {
        var success =  await database.DeleteElement<Yarn>(request.Id);
        
        return success is { IsAcknowledged: true, DeletedCount: > 0 };
    }
}