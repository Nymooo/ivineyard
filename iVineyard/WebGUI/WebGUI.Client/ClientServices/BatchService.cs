using System.Net.Http.Json;
using Model.Entities.Harvest;

namespace WebGUI.Client.ClientServices;

public class BatchService
{
    private readonly HttpClient _httpClient;

    public BatchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Batch>?> GetBatchesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Batch>>("http://localhost:5189/batches/batches");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<HttpResponseMessage?> CreateBatchAsync(object batch) {
        try
        {
            return await _httpClient.PostAsJsonAsync($"http://localhost:5189/batches/create", batch);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
}