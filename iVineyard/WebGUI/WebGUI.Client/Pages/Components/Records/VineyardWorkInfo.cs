namespace WebGUI.Client.Pages.Components.Records;

public record VineyardWorkInfo()
{
    public string VineyardName { get; set; }
    public List<string> MachineNames { get; set; } = new();
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}