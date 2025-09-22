namespace Model.Entities.Harvest;

public class WineBatchHasTreatment
{
    public Batch Batch { get; set; }
    public int BatchId { get; set; }

    public Treatment Treatment { get; set; }
    public int TreatementId { get; set; }

    public double Amount { get; set; }
    public string Agent { get; set; }
    public DateTime Date { get; set; }
}