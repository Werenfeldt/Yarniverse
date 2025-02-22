using Database;
using Domain;
using MediatR;

namespace Application;

public record CreateYarnCommand(List<string> ProducerNames, List<string> YarnNames, List<int> Gauges, List<double> NeedleSize) : IRequest<bool> { }

public class CreateYarnHandler(IMongoDb database) : IRequestHandler<CreateYarnCommand, bool>
{
    public async Task<bool> Handle(CreateYarnCommand request, CancellationToken cancellationToken)
    {
        List<Yarn> yarns = new List<Yarn>();
        for (int i = 0; i < request.ProducerNames.Count; i++)
        {
            var yarn = new Yarn(
                request.ProducerNames[i],
                new Producer(
                    request.YarnNames[i]),
                "Green",
                new Gauge(
                    request.Gauges[i], 
                    request.NeedleSize[i]));
            
            yarns.Add(yarn);
        }

        return await database.InsertElements(yarns);
    }
}
