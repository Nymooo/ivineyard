using Microsoft.AspNetCore.Identity;
using Model.Configurations;

namespace WebGUI.Client.Pages.Components.Records;

public record UserWithRoleRecord()
{
    public ApplicationUser UserData { get; set; } = new();
    public IdentityRole? RoleData { get; set; } = new();
}