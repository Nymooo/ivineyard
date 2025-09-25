using System.Net.Http.Json;
using Model.Entities.Harvest;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class VineyardHasBatchService
{
    private readonly HttpClient _httpClient;

    public VineyardHasBatchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<BatchInformationRecord>?> GetVineyardsWithBatchesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BatchInformationRecord>>("http://localhost:5189/vineyardbatches/vineyardbatches");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<BatchInformationRecord?> GetVineyardsWithBatchesByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BatchInformationRecord>($"http://localhost:5189/vineyardbatches/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
}