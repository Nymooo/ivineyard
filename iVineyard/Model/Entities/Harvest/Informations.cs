namespace Model.Entities.Harvest;

public class Informations
{
    public int InformationId { get; set; }

    public Batch Batch { get; set; }
    public int BatchId { get; set; }

    public DateTime Date { get; set; }
    public double Acidity { get; set; }
    public double PhValue { get; set; }
    public string FurtherSteps { get; set; }    
}