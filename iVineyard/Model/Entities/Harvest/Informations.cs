using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;

[Table("INFORMATIONS")]
public class Informations
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("INFORMATION_ID")]
    public int InformationId { get; set; }
    [Column("BATCH")]
    public Batch? Batch { get; set; }
    [Column("BATCH_ID")]
    public int BatchId { get; set; }
    [Column("DATE")]
    public DateTime Date { get; set; }
    [Column("ACIDITY")]
    public string Acidity { get; set; }
    [Column("PH_VALUE")]
    public string PhValue { get; set; }
    [Column("FURTHER_STEPS")]
    public string FurtherSteps { get; set; }

    public StartingMust? StartingMust { get; set; }
    public YoungWine? YoungWine { get; set; }
    public WhiteWine_RedWine? WhiteWineRedWine { get; set; }
}