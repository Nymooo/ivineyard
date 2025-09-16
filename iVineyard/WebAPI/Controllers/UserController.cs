using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Configurations;
using Model.Entities.Bookingobjects;
using WebGUI.Client.Pages.Components.Records;

namespace WebAPI.Controllers;

[ApiController]
[Route("users")]
public class UserController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RoleController> logger, IBookingObjectRepository BookingObjectRepository) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<ApplicationUser>> CreateUser(UserCreateRecord newUser)
    {
        var user = new ApplicationUser
        {
            UserName = newUser.UserData.Email,
            Email = newUser.UserData.Email,
            Salary = newUser.UserData.Salary,
        };

        var bookingObject = await BookingObjectRepository.CreateAsync(new BookingObject());
        user.BookingObjectId = bookingObject.Id;
        var createUser = await userManager.CreateAsync(user, newUser.Password);
        await userManager.AddToRoleAsync(user, newUser.RoleData.Name);
        
        if (!createUser.Succeeded)
        {
            logger.LogError("Failed to create user: {Errors}", createUser.Errors);
            return BadRequest(createUser.Errors);
        }
        
        logger.LogInformation("User {Email} created successfully", user.Email);
        return Ok(new { message = "User created successfully", user.Id });
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(string id, [FromBody] UserUpdateRecord updateData)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user is null)
        {
            logger.LogInformation($"no data for update found: {id}");
            return NoContent();
        }
        
        user.Email = updateData.UserData.Email;
        user.Salary = updateData.UserData.Salary;
        
        // Remove all roles from User
        await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        
        // Add new Role
        await userManager.AddToRoleAsync(user, updateData.RoleData.Name);
        
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            logger.LogInformation($"data updated: {updateData}");
            return Ok();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            logger.LogWarning("User with ID {Id} not found", id);
            return NotFound();
        }
        
        var roles = await userManager.GetRolesAsync(user);
        if (roles.Any())
        {
            var removeRolesResult = await userManager.RemoveFromRolesAsync(user, roles);
            if (!removeRolesResult.Succeeded)
            {
                logger.LogError("Failed to remove roles from user {Id}: {Errors}", id, removeRolesResult.Errors);
                return BadRequest(removeRolesResult.Errors);
            }
        }
        
        var deleteResult = await userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            logger.LogError("Failed to delete user {Id}: {Errors}", id, deleteResult.Errors);
            return BadRequest(deleteResult.Errors);
        }
        
        logger.LogInformation("User with ID {Id} deleted successfully", id);
        return Ok(new { message = "User deleted successfully" });
    }
    
    [HttpGet("employees")]
    public async Task<ActionResult<List<ApplicationUser>>> GetAllEmployees()
    {
        var employees = new List<ApplicationUser>();
        var users = userManager.Users.ToList();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (roles.Any(x => x.Equals("Employee")))
                employees.Add(user);
        }

        if (employees is null)
        {
            logger.LogInformation($"roles not found");
            return NotFound();
        }

        logger.LogInformation($"roles found {employees}");
        return Ok(employees);
    }
    
    [HttpGet("users")]
    public async Task<ActionResult<List<ApplicationUser>>> GetAllUsers()
    {
        var users = userManager.Users.ToList();

        if (users is null)
        {
            logger.LogInformation($"users not found");
            return NotFound();
        }

        logger.LogInformation($"users found {users}");
        return Ok(users);
    }
    
    [HttpGet("user-role/{id}")]
    public async Task<ActionResult<UserWithRoleRecord>> GetUserWithRole(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
        {
            logger.LogInformation("User with ID {Id} not found", id);
            return NotFound(new { message = $"User with ID {id} not found" });
        }
        
        var roleNames = await userManager.GetRolesAsync(user);
        var roles = new List<IdentityRole?>();
        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                roles.Add(role);
            }
        }
        
        // Create a response object
        var userWithRoles = new UserWithRoleRecord()
        {
            UserData = user,
            RoleData = roles.FirstOrDefault()
        };
        
        logger.LogInformation("User with ID {Id} and roles retrieved successfully", id);
        return Ok(userWithRoles);
    }
    
    [HttpGet("user-role")]
    public async Task<ActionResult<List<UserWithRoleRecord>>> GetUsersWithRole()
    {
        var users = userManager.Users.ToList(); // Retrieve all users

        if (!users.Any())
        {
            logger.LogInformation("No users found");
            return NotFound(new { message = "No users found" });
        }

        var usersWithRoles = new List<UserWithRoleRecord>();

        foreach (var user in users)
        {
            var roleNames = await userManager.GetRolesAsync(user); // Get roles for the user
            var roles = new List<IdentityRole?>();

            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName); // Retrieve role details
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            usersWithRoles.Add(new UserWithRoleRecord
            {
                UserData = user,
                RoleData = roles.FirstOrDefault() // Assuming you only need the first role
            });
        }

        logger.LogInformation("Users with their roles retrieved successfully");
        return Ok(usersWithRoles);
    }
}