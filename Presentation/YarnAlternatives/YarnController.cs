using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YarnAlternatives;

[ApiController]
[Route("api/[controller]")]
public class YarnController(ILogger<YarnController> logger, IMediator mediator) : Controller
{
    // GET
    [HttpPost]
    public async Task<IActionResult> CreateYarn()
    {
        List<string> producers = ["Sandness", "Filcolana", "Hobbii", "Hjeltsholm"];
        List<string> yarnNames = ["Double Sunday", "Highland", "Fluffy", "Workeryarn"];
        List<int> gauges = [22, 25, 28, 20];
        List<double> needles = [4.5, 5, 7, 3.5];
        
        var result = await mediator.Send(new CreateYarnCommand(producers, yarnNames, gauges, needles));
        
        logger.LogInformation($"Inserted Yarn with result: {result}");
        return Ok(result);
    }
    
    // GET
    [HttpDelete]
    public async Task<IActionResult> DeleteYarn(string yarnId)
    {
        var deleted = await mediator.Send(new DeleteYarnCommand(yarnId));
        logger.LogInformation("Deleted Yarn");
        
        if (!deleted)
        {
            return BadRequest();
        }

        return Ok();
    }
}