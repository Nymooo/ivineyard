using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Model.Configurations;
using Model.Entities.Bookingobjects;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class UserService(HttpClient httpClient)
{
    public async Task<HttpResponseMessage?> CreateUserAsync(string password, ApplicationUser userData, IdentityRole role)
    {
        try
        {
            var createRequest = new UserCreateRecord()
            {
                UserData = userData,
                RoleData = role,
                Password = password
            };
            
            return await httpClient.PostAsJsonAsync($"http://localhost:5189/users/create", createRequest);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    public async Task<HttpResponseMessage?> UpdateUser(string id, ApplicationUser userData, IdentityRole newRole)
    {
        try
        {
            var updateRequest = new UserUpdateRecord()
            {
                UserData = userData,
                RoleData = newRole
            };
            
            return await httpClient.PutAsJsonAsync($"http://localhost:5189/users/{id}", updateRequest);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }

    public async Task<HttpResponseMessage?> DeleteUser(ApplicationUser userData)
    {
        try
        {
            return await httpClient.DeleteFromJsonAsync<HttpResponseMessage>($"http://localhost:5189/users/{userData.Id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    public async Task<List<ApplicationUser>?> GetAllEmployees()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<ApplicationUser>>("http://localhost:5189/users/employees");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<ApplicationUser>?> GetAllUsers()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<ApplicationUser>>("http://localhost:5189/users/users");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<UserWithRoleRecord?> GetUserWithRole(string id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<UserWithRoleRecord>($"http://localhost:5189/users/user-role/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<UserWithRoleRecord>?> GetUsersWithRole()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<UserWithRoleRecord>>($"http://localhost:5189/users/user-role");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
}