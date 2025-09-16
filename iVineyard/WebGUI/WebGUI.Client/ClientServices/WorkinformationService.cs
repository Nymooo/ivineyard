using System.Net.Http.Json;
using Model.Entities.Bookingobjects.Vineyard;
using WebGUI.Client.Pages.Components.Records;

namespace WebGUI.Client.ClientServices;

public class WorkinformationService {
    private readonly HttpClient _httpClient;

    public WorkinformationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<WorkInformation>> GetVineyardInfobyIdAsync(int id) {
        try
        {
            List<WorkInformation> workInformation = await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/vineyard/{id}");
            return workInformation;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<HttpResponseMessage?> CreateVineyardInfoAsync(List<WorkInformation> newWorkInformation) {
        try
        {
            return await _httpClient.PostAsJsonAsync($"http://localhost:5189/workinfo/create", newWorkInformation);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<List<WorkInformation>?> GetWorkInformation() {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/workinfo");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<List<WorkInformation>?> GetWorkInformationMonth() {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/workinfo/month");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<WorkInformation>?> GetWorkInformationToday() {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/workinfo/today");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<WorkInformation>?> GetUserWorkInformation(string id) {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/workinfo/user/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<List<WorkInformation>?> GetUserWorkInformationMonth(string id) {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<WorkInformation>>($"http://localhost:5189/workinfo/workinfo/user-monthly/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public List<UserWorkInfo> GroupWorkInformation(List<WorkInformation> workInfoList)
    {
        return workInfoList
            .GroupBy(wi => wi.ApplicationUser.Id) // Gruppierung nach Benutzer
            .Select(userGroup => new UserWorkInfo
            {
                UserId = userGroup.Key,
                UserEmail = userGroup.First().ApplicationUser.Email,
                Vineyards = userGroup
                    .GroupBy(wi => wi.Vineyard.Name) // Gruppierung nach Weingarten
                    .Select(vineyardGroup => new VineyardWorkInfo
                    {
                        VineyardName = vineyardGroup.Key,
                        MachineNames = vineyardGroup
                            .Where(wi => wi.Machine != null) // Entfernt null-Einträge
                            .Select(wi => wi.Machine.Name)
                            .Distinct()
                            .ToList(),
                        StartedAt = vineyardGroup.First().StartedAt,
                        FinishedAt = vineyardGroup.First().FinishedAt
                    }).ToList()
            }).ToList();

    }

    public double GetTodaysExpenses(List<WorkInformation> workInfo)
    {
        return workInfo.Where(wi => wi.ApplicationUser != null)
            .Sum(wi =>
            {
                var durationHours = wi.FinishedAt.HasValue
                    ? (wi.FinishedAt.Value - wi.StartedAt.Value).TotalHours
                    : (DateTime.Now - wi.StartedAt.Value).TotalHours;

                return Math.Round(durationHours * wi.ApplicationUser.Salary, 2);
            });
    }
    
    public List<WorkInformation> FilterOngoingWorkInformation(List<WorkInformation>? workInfo)
    {
        var currentDate = DateTime.Now;
        if (workInfo != null && workInfo.Any())
        {
            return workInfo
                .Where(w => w.StartedAt <= currentDate && w.FinishedAt >= currentDate)
                .ToList();
        }
        return new List<WorkInformation>();
    }
    
    public bool CheckIfCurrentDateIsInWorkRange(List<WorkInformation>? workInfo)
    {
        if (workInfo != null && workInfo.Any())
        {
            var currentDate = DateTime.Now;
            foreach (var work in workInfo)
            {
                if (work.StartedAt <= currentDate && work.FinishedAt >= currentDate)
                {
                    return true;
                }
            }
        }
        return false;
    }
}