using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("roles")]
public class RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger) : ControllerBase
{

    [HttpGet("roles")]
    public async Task<ActionResult<List<IdentityRole>>> GetAllRoles()
    {
        var roles = roleManager.Roles.ToList();

        if (roles is null)
        {
            logger.LogInformation($"roles not found");
            return NotFound();
        }
        
        logger.LogInformation($"roles found {roles}");
        return Ok(roles);
    }
}