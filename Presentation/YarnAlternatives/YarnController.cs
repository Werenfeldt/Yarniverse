using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YarnAlternatives;

[ApiController]
[Route("api/[controller]")]
public class YarnController(ILogger<YarnController> logger, IMediator mediator) : Controller
{

    [HttpPost]
    public async Task<IActionResult> CreateYarn()
    {
        List<string> producers = ["Sandness", "Filcolana", "Hobbii", "Hjeltsholm", "Sandness"];
        List<string> yarnNames = ["Double Sunday", "Highland", "Fluffy", "Workeryarn", "Peer Gynt"];
        List<int> gauges = [22, 25, 28, 20, 20];
        List<double> needles = [4.5, 5, 7, 3.5, 4];
        
        var result = await mediator.Send(new CreateYarnCommand(producers, yarnNames, gauges, needles));
        
        logger.LogInformation($"Inserted Yarn with result: {result}");
        return Ok(result);
    }
    
    [HttpPost("AddSingle")]
    public async Task<IActionResult> AddSingleYarn(string producer, string yarnName, int gauge, double needle)
    {
        List<string> producers = [producer];
        List<string> yarnNames = [yarnName];
        List<int> gauges = [gauge];
        List<double> needles = [needle];
        
        var result = await mediator.Send(new CreateYarnCommand(producers, yarnNames, gauges, needles));
        
        logger.LogInformation($"Inserted Yarn with result: {result}");
        return Ok(result);
    }
    
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

    [HttpGet("FindAlternatives")]
    public async Task<IActionResult> FindAlternative(int gauge, double needle)
    {
        var result = await mediator.Send(new FindAlternativeSameNeedleOneThread(gauge, needle));

        if (result.Count is 0)
        {
            return NotFound();
        }
        
        return Ok(result);
    }
}