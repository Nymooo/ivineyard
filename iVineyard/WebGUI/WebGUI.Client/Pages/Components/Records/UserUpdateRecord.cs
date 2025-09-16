using Microsoft.AspNetCore.Identity;
using Model.Configurations;

namespace WebGUI.Client.Pages.Components.Records;

public record UserUpdateRecord()
{
    public ApplicationUser UserData { get; set; } = new();
    public IdentityRole RoleData { get; set; } = new();
}