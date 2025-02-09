using Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace YarnAlternatives;

[ApiController]
[Route("api/[controller]")]
public class YarnController(ILogger<YarnController> logger, IMongoDb database) : Controller
{
    // GET
    [HttpGet]
    public async Task<IActionResult> GetYarn()
    {
        var result =  await database.InsertElement();
        logger.LogInformation("Inserted Yarn");
        return Ok(result);
    }
    
    // GET
    [HttpDelete]
    public async Task<IActionResult> DeleteYarn(string yarnId)
    {
        var success =  await database.DeleteElement(yarnId);
        logger.LogInformation("Inserted Yarn");
        if (success.IsAcknowledged)
        {
            return Ok(success.ToJson());
        }
        else
        {
            return BadRequest(success.ToJson());
        }
    }
}