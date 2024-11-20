using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Obama.Infrastructure;

namespace Obama.Controllers;

[Route("odata")] 
public class DefaultController(ObamaContext context, ILogger<DefaultController> logger) : ODataController
{
    [HttpGet("GetHighestBonus()")]
    public ActionResult<decimal> GetHighestBonus()
    {
        try
        {
            return context.Employees.Max(e => e.Bonus);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get highest salary");
            return ODataErrorResult("500", "Failed to get highest salary");
        }
    }   
    
    [HttpPost("ConferBonuses()")]
    public async Task<IActionResult> ConferBonuses(ODataActionParameters? parameters)
    {
        try
        {
            if (parameters is null || !parameters.TryGetValue("Bonus", out var bonus)) return BadRequest("The 'Bonus' parameter is required.");
            
            foreach (var employee in context.Employees) employee.ConferBonus(Convert.ToDecimal(bonus));
            await context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError("Failed to confer bonuses");
            return StatusCode(500, $"An unexpected error occurred: {exception.Message}");
        }
    }
}