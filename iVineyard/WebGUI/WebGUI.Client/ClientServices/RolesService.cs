using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;

namespace WebGUI.Client.ClientServices;

public class RolesService(HttpClient httpClient)
{
    public async Task<List<IdentityRole>?> GetRolesAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<IdentityRole>>("http://localhost:5189/roles/roles");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
}