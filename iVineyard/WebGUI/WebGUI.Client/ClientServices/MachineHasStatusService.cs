using System.Net.Http.Json;

namespace WebGUI.Client.ClientServices;

public class MachineHasStatusService {
    private readonly HttpClient _httpClient;

    public MachineHasStatusService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task CreateVineyardhasStatus(int machineid, int statusid) {
        int[] i = new int[2];
        i[0] = statusid;
        i[1] = machineid;
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5189/machineHasStatus/create", i);
    }
}