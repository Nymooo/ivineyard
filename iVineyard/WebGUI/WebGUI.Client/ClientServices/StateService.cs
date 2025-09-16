using System.Net.Http.Json;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebGUI.Client.ClientServices;

public class StateService {
    
    private readonly HttpClient _httpClient;

    public StateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<VineyardStatusType>?> GetState()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<VineyardStatusType>>("http://localhost:5189/state/states");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
}