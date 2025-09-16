using System.Net.Http.Json;
using Model.Entities.Bookingobjects;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class BookingObjectService {
    private readonly HttpClient _httpClient;

    public BookingObjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<BookingObject> ManageBookingObject()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BookingObject>("http://localhost:5189/booking/manage");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
}