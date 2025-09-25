using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Entities.Harvest;
[Table("STARTING_MUST")]
public class StartingMust
{
    [JsonIgnore]
    public Informations? Informations { get; set; }
    [Column("INFORMATION_ID"), Required]
    public int Id { get; set; }
    
   [Column("KMW/OE")]
    public double KMW_OE { get; set; }
    [Column("REBEL")]
    public string Rebel { get; set; }
    [Column("SQUEEZE")]
    public string Squeeze { get; set; }
    [Column("MASH_LIFE")]
    public string MashLife { get; set; }
}