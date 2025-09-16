using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;
using Model.Entities;
using Model.Entities.Bookingobjects;

namespace WebGUI.Client.ClientServices;

public class WeatherService {
    private readonly HttpClient _httpClient;
    DateTime? date2 = DateTime.Today; // Variable zur Speicherung des ausgewählten Datums, standardmäßig auf das heutige Datum gesetzt
    List<Measurement>? MeasurementData = new();
    
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<Measurement>?> WeatherData()
    {
        try
        {
            // Formatieren des Datums
            string formattedDate = date2.Value.ToString("yyyy-MM-dd") ?? throw new InvalidOperationException("Date is null or invalid");

            // API-Anfrage
            var response = await _httpClient.GetAsync($"https://api.ivineyard.eu/api.php?stationId=1&authToken=testToken&date={formattedDate}");
            Console.WriteLine($"Response Status Code: {response.StatusCode}");

            // Prüfen, ob die Anfrage erfolgreich war
            if (response.IsSuccessStatusCode)
            {
                // Lesen der Antwort
                MeasurementData = await response.Content.ReadFromJsonAsync<List<Measurement>>();
                if (MeasurementData != null)
                {
                    Console.WriteLine("Data successfully fetched.");
                    return MeasurementData;
                }
                else
                {
                    Console.WriteLine("No data received from API.");
                    return null;
                }
            }
            else
            {
                // Fehler-Logging
                Console.WriteLine($"API call failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return null;
            }
        }
        catch (HttpRequestException httpEx)
        {
            // Netzwerkfehler behandeln
            Console.WriteLine($"Network error occurred: {httpEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Allgemeine Fehler behandeln
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
    
}