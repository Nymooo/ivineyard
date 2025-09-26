using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("WINE_BATCH_has_TREATMENT")]
public class WineBatchHasTreatment
{
    public Batch? Batch { get; set; }
    [Column("BATCH_ID")]
    public int BatchId { get; set; }

    public Treatment? Treatment { get; set; }
    [Column("TREATMENT_ID")]
    public int TreatementId { get; set; }
    [Column("AMOUNT")]
    public string Amount { get; set; }
    [Column("AGENT")]
    public string Agent { get; set; }
    [Column("DATE")]
    public DateTime Date { get; set; }
}