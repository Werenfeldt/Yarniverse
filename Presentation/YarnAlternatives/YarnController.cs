using Application.AlgorithmStrategies;
using Application.Commands;
using Application.Model;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace YarnAlternatives;

[ApiController]
[Route("api/[controller]")]
public class YarnController(ILogger<YarnController> logger, IYarnService yarnService, IYarnAlternativeService yarnAlternativeService) : Controller
{

    [HttpPost]
    public async Task<IActionResult> CreateYarn()
    {
        List<string> producers = ["Sandness", "Filcolana", "Hobbii", "Hjeltsholm", "Isager"];
        List<string> yarnNames = ["Double Sunday", "Highland", "Fluffy", "Workeryarn", "Jensen Yarn"];
        List<StitchRange> gauges = [new (22, 24), new (25, 26), new (28, 30), new (20, 21), new(20, 23)];
        List<NeedleRange> needles = [new (4.5, 5.0), new (5, 6.5), new (7, 8), new (3.5, 4.5), new (4, 4)];
        
        var result = await yarnService.CreateYarn(new CreateYarnCommand(producers, yarnNames, gauges, needles));
        
        logger.LogInformation($"Inserted Yarn with result: {result}");
        return Ok(result);
    }
    
    [HttpPost("AddSingle")]
    
    public async Task<IActionResult> AddSingleYarn([FromBody] CreateYarnCommand request)
    {
        
        var result = await yarnService.CreateYarn(request);
        
        logger.LogInformation($"Inserted Yarn with result: {result}");
        return Ok(result);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteYarn(string yarnId)
    {
        var deleted = await yarnService.DeleteYarn(yarnId);
        logger.LogInformation("Deleted Yarn");
        
        if (!deleted.IsSuccess)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpGet("FindAlternatives")]
    public async Task<IActionResult> FindAlternative(int gauge, double needle)
    {
        var result = await yarnAlternativeService.GetYarnAlternatives(new FindAlternativeSameNeedleOneThread(gauge, needle));

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }
        
        return Ok(result);
    }
}