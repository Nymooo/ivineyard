using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Model.Entities.Bookingobjects.Machine;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class MachineService(HttpClient httpClient)
{
    public async Task<List<Machine>?> GetMachinesAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<Machine>>("http://localhost:5189/machines/machines");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<MachineRecord>?> GetMachinesInfoAsync()
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<MachineRecord>>("http://localhost:5189/machines/machinesinfo");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    
    
    public async Task<Machine?> GetMachineAsync(int id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<Machine>($"http://localhost:5189/machines/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }

    public async Task CreateMachineAsync(string name) {
        try
        {
            await httpClient.PostAsJsonAsync("http://localhost:5189/machines/create",name);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
           
        }
    }

    public async Task DeleteMachineAsync(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"http://localhost:5189/machines/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to delete machine. Status Code: {response.StatusCode}");
                Console.WriteLine($"Response Content: {content}");
            }
            else
            {
                Console.WriteLine($"Machine with ID {id} deleted successfully.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }
      
}