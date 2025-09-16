using Microsoft.AspNetCore.Identity;
using Model.Configurations;

namespace WebGUI.Client.Pages.Components.Records;

public class UserCreateRecord
{
    public ApplicationUser UserData { get; set; } = new();
    public string Password { get; set; } = String.Empty;
    public IdentityRole RoleData { get; set; } = new();
}