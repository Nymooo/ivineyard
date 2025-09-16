using System.Net.Http.Json;
using Model.Entities.Bookingobjects.Vineyard;
using Model.Entities.Company;
using MudBlazor;

namespace WebGUI.Client.ClientServices;

public class FinanceService
{
    private readonly HttpClient _httpClient;
    private readonly WorkinformationService _workinformationService;

    public double CurrentMonthIncome { get; private set; }
    public double CurrentMonthExpenses { get; private set; }
    public double CurrentMonthProfit { get; private set; }
    public double TotalCapital { get; private set; }
    
    public List<Invoice> Invoices { get; private set; } = new();
    public List<Invoice> UserInvoices { get; private set; } = new();
    public List<Invoice> VineyardInvoices { get; private set; } = new();
    public List<Invoice> MachineInvoices { get; private set; } = new();
    public List<Invoice> EquipmentInvoices { get; private set; } = new();
    public List<Invoice> OtherInvoices { get; private set; } = new();

    public double[] Data { get; private set; } = Array.Empty<double>();
    public string[] Labels { get; private set; } = Array.Empty<string>();
    public List<ChartSeries> Series { get; private set; } = new();
    public string[] XAxisLabels { get; private set; } = new string[]
    {
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    };

    private List<WorkInformation>? usersMonthWork = new();
    private List<WorkInformation>? usersWholeWork = new();
    
    public FinanceService(HttpClient httpClient, WorkinformationService workinformationService)
    {
        _httpClient = httpClient;
        _workinformationService = workinformationService;
    }
    
    #region APICalls
    
    public async Task<List<Invoice>?> GetInvoicesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Invoice>>("http://localhost:5189/finances/invoices");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;
        }
    }
    
    public async Task<HttpResponseMessage?> CreateInvoiceAsync(Invoice newInvoice) {
        try
        {
            return await _httpClient.PostAsJsonAsync($"http://localhost:5189/finances/create", newInvoice);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<HttpResponseMessage?> DeleteInvoiceAsync(int id) {
        try
        {
            return await _httpClient.DeleteFromJsonAsync<HttpResponseMessage>($"http://localhost:5189/finances/delete/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    public async Task<HttpResponseMessage?> UpdateInvoiceAsync(int id, Invoice updateInvoice) {
        try
        {
            return await _httpClient.PutAsJsonAsync($"http://localhost:5189/finances/update/{id}", updateInvoice);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return null;    
        }
    }
    
    #endregion
    
    public async Task LoadFinanceDataAsync()
    {
        Invoices = await GetInvoicesAsync();
        usersMonthWork = await _workinformationService.GetWorkInformationMonth();
        usersWholeWork = await _workinformationService.GetWorkInformation();
        
        SortInvoicesByType();
        CalculateFinanceMetrics(Invoices, usersMonthWork, usersWholeWork);
        PrepareChartData();
    }
    
    private void SortInvoicesByType()
    {
        UserInvoices.Clear();
        VineyardInvoices.Clear();
        MachineInvoices.Clear();
        EquipmentInvoices.Clear();
        OtherInvoices.Clear();

        foreach (var invoice in Invoices)
        {
            switch (invoice.BookingObject)
            {
                case { ApplicationUser: not null }:
                    UserInvoices.Add(invoice);
                    break;
                case { Vineyard: not null }:
                    VineyardInvoices.Add(invoice);
                    break;
                case { Machine: not null }:
                    MachineInvoices.Add(invoice);
                    break;
                case { Equipment: not null }:
                    EquipmentInvoices.Add(invoice);
                    break;
                default:
                    OtherInvoices.Add(invoice);
                    break;
            }
        }
    }
    
    private void CalculateFinanceMetrics(IEnumerable<Invoice> invoices, IEnumerable<WorkInformation> usersMonthWork, IEnumerable<WorkInformation> usersWholeWork)
    {
        var currentMonth = DateTime.Now.Month;
        CurrentMonthIncome = invoices.Where(i => i.Price > 0 && i.BoughAt?.Month == currentMonth)
            .Sum(i => i.Price);

        double monthWorkCosts = usersMonthWork?.Sum(work => CalculateWorkCost(work)) ?? 0;
        CurrentMonthExpenses = Math.Abs(invoices.Where(i => i.Price < 0 && i.BoughAt?.Month == currentMonth)
            .Sum(i => i.Price) - monthWorkCosts);

        CurrentMonthProfit = CurrentMonthIncome - CurrentMonthExpenses;
        
        double totalWorkCosts = usersWholeWork?.Sum(work => CalculateWorkCost(work)) ?? 0;
        var totalIncome = invoices.Where(i => i.Price > 0).Sum(i => i.Price);
        var totalExpenses = Math.Abs(invoices.Where(i => i.Price < 0).Sum(i => i.Price) - totalWorkCosts);
        TotalCapital = totalIncome - totalExpenses;
    }
    
    private void PrepareChartData()
    {
        var expenseTypes = new List<(string Type, double Amount)>
        {
            ("User Expenses", Math.Abs(UserInvoices.Where(i => i.Price < 0).Sum(i => i.Price))),
            ("Vineyard Expenses", Math.Abs(VineyardInvoices.Where(i => i.Price < 0).Sum(i => i.Price))),
            ("Machine Expenses", Math.Abs(MachineInvoices.Where(i => i.Price < 0).Sum(i => i.Price))),
            ("Equipment Expenses", Math.Abs(EquipmentInvoices.Where(i => i.Price < 0).Sum(i => i.Price))),
            ("Other Expenses", Math.Abs(OtherInvoices.Where(i => i.Price < 0).Sum(i => i.Price)))
        };
        
        expenseTypes = expenseTypes.Where(e => e.Amount > 0).ToList();
        Data = expenseTypes.Select(e => e.Amount).ToArray();
        Labels = expenseTypes.Select(e => e.Type).ToArray();

        var incomeData = Enumerable.Range(1, 12)
            .Select(month => Invoices
                .Where(i => i.Price > 0 && i.BoughAt?.Month == month)
                .Sum(i => i.Price))
            .ToArray();


        // var expenseData = Invoices.Where(i => i.Price < 0)
        //     .GroupBy(i => i.BoughAt?.Month)
        //     .Select(g => Math.Abs(g.Sum(i => i.Price)))
        //     .ToArray();
        //
        // var userCostMonth = Enumerable.Range(1, 12)
        //     .Select(month => usersWholeWork
        //         .Where(w => w.StartedAt?.Month == month)
        //         .Sum(w => CalculateWorkCost(w)))
        //     .ToArray();
        
        var combinedExpenses = Enumerable.Range(1, 12)
            .Select(month => 
                Math.Abs(Invoices
                    .Where(i => i.Price < 0 && i.BoughAt?.Month == month)
                    .Sum(i => i.Price)) +
                usersWholeWork
                    .Where(w => w.StartedAt?.Month == month)
                    .Sum(w => CalculateWorkCost(w))
            ).ToArray();
        
        
        var profitData = incomeData.Zip(combinedExpenses, (income, expense) => income - expense).ToArray();
        
        Series = new List<ChartSeries>
        {
            new ChartSeries() { Name = "Income", Data = incomeData },
            new ChartSeries() { Name = "Expenses", Data = combinedExpenses },
            new ChartSeries() { Name = "Profit", Data = profitData }
        };
    }

    private double CalculateWorkCost(WorkInformation work)
    {
        var hoursWorked = (work.FinishedAt.Value - work.StartedAt.Value).TotalHours;
        return hoursWorked * work.ApplicationUser.Salary;
    }
}