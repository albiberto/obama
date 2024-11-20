using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Obama.Domain;
using Obama.Infrastructure;

namespace Obama.Controllers;

public class RolesController(ObamaContext context, ILogger<RolesController> logger) : ODataController
{
    [EnableQuery]
    public ActionResult<IEnumerable<Role>> Get()
    {
        try
        {
            return Ok(context.Roles);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get roles");
            return ODataErrorResult("500", "Failed to get roles");
        }
    }

    [EnableQuery]
    public async Task<ActionResult<Role>> Get([FromRoute] Guid key)
    {
        try
        {
            var role = await context.Roles.FindAsync(key);

            return role is null
                ? NotFound("Role not found")
                : Ok(role);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get role");
            return ODataErrorResult("500", "Failed to get role");
        }
    }

    public async Task<ActionResult> Post([FromBody] Role role)
    {
        if (role is null) return BadRequest("Role cannot be null");

        try
        {
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();

            return Created(role);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create role");
            return ODataErrorResult("500", "Failed to create role");
        }
    }

    public async Task<ActionResult> Put([FromRoute] Guid key, [FromBody] Role updatedRole)
    {
        if (updatedRole is null) return BadRequest("Role cannot be null");

        try
        {
            var role = await context.Roles.FindAsync(key);
            if (role is null) return NotFound("Role not found");

            role.Name = updatedRole.Name;
            role.Enabled = updatedRole.Enabled;

            await context.SaveChangesAsync();

            return Updated(role);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update role");
            return ODataErrorResult("500", "Failed to update role");
        }
    }

    public async Task<ActionResult> Patch([FromRoute] Guid key, [FromBody] Delta<Role> delta)
    {
        if (delta is null) return BadRequest("Role cannot be null");

        try
        {
            var role = await context.Roles.FindAsync(key);

            if (role is null) return NotFound("Role not found");

            delta.Patch(role);

            await context.SaveChangesAsync();

            return Updated(role);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update role");
            return ODataErrorResult("500", "Failed to update role");
        }
    }

    public async Task<ActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            var role = await context.Roles.FindAsync(key);

            if (role is null) return NotFound("Role not found");

            context.Roles.Remove(role);
            await context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to delete role");
            return ODataErrorResult("500", "Failed to delete role");
        }
    }
}