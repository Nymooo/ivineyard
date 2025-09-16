using System.Net.Http.Json;
using Model.Entities.Bookingobjects.Vineyard;

namespace WebGUI.Client.ClientServices;

public class VineyardHasStatusService {
    private readonly HttpClient _httpClient;

    public VineyardHasStatusService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CreateVineyardhasStatus(Dictionary<string, object> vineyardStatusData) {
        var vineyardhasStatus = new VineyardHasStatus() {
            VineyardId = (int)vineyardStatusData["VineyardId"],
            StartDate = (DateTime)vineyardStatusData["StartDate"],
            StatusId = (int)vineyardStatusData["StatusId"],
            EndDate = null
        };
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5189/vineyardhasstatus/create", vineyardhasStatus);
    }

    public async Task UpdateVineyardhasStatus(VineyardHasStatus x)
    {
        
        Console.WriteLine($"Status {x.StatusId} wird aktualisiert...SERVICE");
        var response = await _httpClient.PutAsJsonAsync("http://localhost:5189/vineyardhasstatus/update", x);
    }

    public async Task<VineyardHasStatus> FindAsync(int vineyardId, int statusId) {
        VineyardHasStatus x= await _httpClient.GetFromJsonAsync<VineyardHasStatus>(
            $"http://localhost:5189/vineyardhasstatus/find?vineyardId={vineyardId}&statusId={statusId}");
        Console.WriteLine("FINDEN VON VINEYARDHASSTATUS");
        Console.WriteLine(x.VineyardId);
        return x;
    }

}