using System.Net.Http.Json;
using Model.Entities.Bookingobjects;

namespace WebGUI.Client.ClientServices;

public class EquipmentService
{
    private readonly HttpClient _httpClient;

    public EquipmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Equipment>?> GetEquipmentAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Equipment>>("http://localhost:5189/equipment/equipment");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<HttpResponseMessage?> CreateEquipmentAsync(Equipment equipment) {
        try
        {
            return await _httpClient.PostAsJsonAsync($"http://localhost:5189/equipment/create", equipment);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
}