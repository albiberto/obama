using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Obama.Domain;
using Obama.Infrastructure;

namespace Obama.Controllers;

public class EmployeesController(ObamaContext context, ILogger<EmployeesController> logger) : ODataController
{
    [EnableQuery]
    public ActionResult<IEnumerable<Employee>> Get()
    {
        try
        {
            return Ok(context.Employees);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get employees");
            return ODataErrorResult("500", "Failed to get employees");
        }
    }

    [EnableQuery]
    public async Task<ActionResult<Employee>> Get([FromRoute] Guid key)
    {
        try
        {
            var employee = await context.Employees.FindAsync(key);

            return employee is null
                ? NotFound("Employee not found")
                : Ok(employee);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get employee");
            return ODataErrorResult("500", "Failed to get employee");
        }
    }

    public async Task<ActionResult> Post([FromBody] Employee? employee)
    {
        if (employee is null) return BadRequest("Employee cannot be null");

        try
        {
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            return Created(employee);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create employee");
            return ODataErrorResult("500", "Failed to create employee");
        }
    }

    public async Task<ActionResult> Put([FromRoute] Guid key, [FromBody] Employee? updatedEmployee)
    {
        if (updatedEmployee is null) return BadRequest("Employee cannot be null");

        try
        {
            var employee = await context.Employees.FindAsync(key);
            if (employee is null) return NotFound("Employee not found");

            employee.FamilyName = updatedEmployee.FamilyName;
            employee.GivenName = updatedEmployee.GivenName;
            employee.Mail = updatedEmployee.Mail;
            employee.RoleId = updatedEmployee.RoleId;

            await context.SaveChangesAsync();

            return Updated(employee);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update employee");
            return ODataErrorResult("500", "Failed to update employee");
        }
    }

    public async Task<ActionResult> Patch([FromRoute] Guid key, [FromBody] Delta<Employee>? delta)
    {
        if (delta is null) return BadRequest("Employee cannot be null");

        try
        {
            var employee = await context.Employees.FindAsync(key);

            if (employee is null) return NotFound("Employee not found");

            delta.Patch(employee);

            await context.SaveChangesAsync();

            return Updated(employee);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update employee");
            return ODataErrorResult("500", "Failed to update employee");
        }
    }

    public async Task<ActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            var employee = await context.Employees.FindAsync(key);

            if (employee is null) return NotFound("Employee not found");

            context.Employees.Remove(employee);
            await context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to delete employee");
            return ODataErrorResult("500", "Failed to delete employee");
        }
    }

    [HttpGet]
    public async Task<ActionResult<int>> GetBonus([FromRoute] Guid key)
    {
        try
        {
            var employee = await context.Employees.FindAsync(key);

            return employee is null
                ? NotFound($"Employee with key '{key}' was not found.")
                : Ok(employee.Bonus);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get bonus");
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult GetEmployeesByRole([FromODataUri] string role)
    {
        if (string.IsNullOrEmpty(role)) return BadRequest("The 'Role' parameter is required.");
        
        try
        {
            var employees = context.Employees
                .Include(e => e.Role)
                .Where(e => e.Role != null && e.Role.Name == role)
                .ToList();

            return Ok(employees);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to get employees by role");
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }

    }

    [HttpPost]
    public async Task<IActionResult> ConferBonus([FromRoute] Guid key, ODataActionParameters? parameters)
    {
        if (parameters is null || !parameters.TryGetValue("Bonus", out var bonus)) return BadRequest("The 'Bonus' parameter is required.");
        
        try
        {
            var employee = await context.Employees.FindAsync(key);

            if (employee is null) 
                return NotFound("Employee not found.");

            employee.ConferBonus(Convert.ToDecimal(bonus));
            await context.SaveChangesAsync();

            return Ok("Bonus successfully assigned.");
        }
        catch (Exception ex)
        { 
            logger.LogError("Failed to confer bonus");
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}