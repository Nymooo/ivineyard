using System.Net.Http.Json;
using Model.Entities.Bookingobjects;
using Model.Entities.Harvest;

namespace WebGUI.Client.ClientServices;

public class TankService
{
    private readonly HttpClient _httpClient;

    public TankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Tank>?> GetTanksAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Tank>>("http://localhost:5189/tanks/tanks");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<HttpResponseMessage?> CreateTankAsync(Tank tank) {
        try
        {
            return await _httpClient.PostAsJsonAsync($"http://localhost:5189/tanks/create", tank);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
}