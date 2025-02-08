using Database;
using Microsoft.AspNetCore.Mvc;

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
}