using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;

[Table("WINE_BATCH")]
public class Batch
{
    [Key]
    public int BatchId { get; set; }
    
    public string Variety { get; set; }
    
    public double Amount { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Maturity_Health { get; set; }
    
    public string Weather { get; set; }
}