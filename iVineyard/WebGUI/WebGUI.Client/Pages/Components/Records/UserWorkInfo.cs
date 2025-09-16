namespace WebGUI.Client.Pages.Components.Records;

public record UserWorkInfo()
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public List<VineyardWorkInfo> Vineyards { get; set; } = new();
}