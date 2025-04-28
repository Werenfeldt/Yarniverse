using Application.AlgorithmStrategies;
using Application.Commands;
using Application.Helpers;
using Application.Model;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using RangeHelper = Application.Helpers.RangeHelper;

namespace YarnAlternatives;

[ApiController]
[Route("api/[controller]")]
public class YarnController(ILogger<YarnController> logger, IYarnService yarnService, IYarnAlternativeService yarnAlternativeService) : Controller
{

    [HttpPost]
    public async Task<IActionResult> CreateYarn()
    {
        List<string> producers = ["Sandness", "Filcolana", "Hobbii", "Hjeltsholm", "Sandness"];
        List<string> yarnNames = ["Double Sunday", "Highland", "Fluffy", "Workeryarn", "Peer Gynt"];
        List<Range<int>> gauges = RangeHelper.MapRange(new List<(int, int)>
        {
            (22, 24), (25, 26), (28, 30), (20, 21), (20, 23)
        });
        List<Range<double>> needles = RangeHelper.MapRange(new List<(double, double)>
        {
            (4.5,5.0), (5,6.5), (7,8), (3.5, 4.5), (4,4)
        });
        
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