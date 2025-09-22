using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;

[Table("WINE_BATCH")]
public class Batch
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BATCH_ID")]
    public int BatchId { get; set; }
    [Column("VARIETY")]
    public string Variety { get; set; }
    [Column("AMOUNT")]
    public double Amount { get; set; }
    [Column("DATE")]
    public DateTime Date { get; set; }
    [Column("MARURITY_HEALTH")]
    public string Maturity_Health { get; set; }
    [Column("WEATHER")]
    public string Weather { get; set; }
}