using Application.Commands;
using Application.Model;
using Database;

namespace Application.Services;

public class YarnService(IMongoDb database) : IYarnService
{
    public async Task<Result> CreateYarn(CreateYarnCommand request, CancellationToken cancellationToken = default)
    {
        List<Yarn> yarns = new List<Yarn>();
        for (int i = 0; i < request.ProducerNames.Count; i++)
        {
            var producerName = request.ProducerNames[i];
            var existingProducerList = await database.GetByPredicateAsync<Yarn>(p => p.Producer.Name == producerName);
            var existingProducer = existingProducerList.FirstOrDefault()?.Producer;
        
            var yarn = new Yarn(
                request.YarnNames[i],
                existingProducer ?? new Producer( 
                    request.ProducerNames[i]),
                new Gauge(
                    request.Gauges[i], 
                    request.NeedleSizes[i])
                );
        
            yarns.Add(yarn);
        }

        return new Result(await database.InsertElements(yarns));
    
    }

    public async Task<Result> DeleteYarn(string id, CancellationToken cancellationToken = default)
    {
        var success =  await database.DeleteElement<Yarn>(id);     
                                     
        return new Result(success);  
    }
}