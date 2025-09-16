using System.Net.Http.Json;
using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class VineyardService
{
    private readonly HttpClient _httpClient;

    public VineyardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Später muss der Port und natürlich die URL generell angepasst werden, da die API
    // nicht immer auf Port 5274 laufen wird. Falls es bei dir nicht geht nochba, schau nach
    // unter welchem port deine API gestartet wird wenn du sie startest und änder es halt um
    
    
    //NOCHBAA der port ist jetzt im programm da dort der port jz definiert wird
    public async Task<List<VineyardRecord>?> GetVineyardsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<VineyardRecord>>("http://localhost:5189/vineyards/vineyards");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }

    public async Task<VineyardRecord> GetVineyardbyIdAsync(int id) {
        try
        {
            VineyardRecord vineyard = await _httpClient.GetFromJsonAsync<VineyardRecord>($"http://localhost:5189/vineyards/{id}");
            return vineyard;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    
    public async Task<bool> CreateVineyard(Dictionary<string, object> vineyardData) {
        var vineyard = new Vineyard() {
            Area = (float) vineyardData["Area"],
            /*CompanyId = 1,*/
            Coordinates = (string) vineyardData["Coordinates"],
            MidCoordinate = (string) vineyardData["MidCoordinate"],
            Name = (string) vineyardData["Name"],
            BookingObjectId = (int)vineyardData["Id"]
        };
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5189/vineyards/create", vineyard);
        if (response.IsSuccessStatusCode) {
            Console.WriteLine(" VINEYARDSERVICE Anfrage war erfolgreich. Statuscode: " + response.StatusCode);
        
            // Optional: Inhalt der Antwort ausgeben, falls die API Daten zurückgibt
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Antwortinhalt: " + responseContent);
            return true;

        } else {
            Console.WriteLine("VINEYARDSERVICE Anfrage fehlgeschlagen. Statuscode: " + response.StatusCode);
        
            // Fehlerdetails anzeigen
            string errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Fehlerantwort: " + errorContent);
            return false;
        }
        
    }
    public async Task DeleteVineyardbyIdAsync(int id) {
        try
        {
            await _httpClient.DeleteAsync($"http://localhost:5189/vineyards/{id}");
            
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            
        }
    }
}