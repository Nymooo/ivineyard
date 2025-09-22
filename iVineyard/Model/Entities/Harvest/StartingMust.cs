using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("STARTING_MUST")]
public class StartingMust : Informations
{
   [Column("KMW/OE")]
    public double KMW_OE { get; set; }
    [Column("REBEL")]
    public string Rebel { get; set; }
    [Column("SQUEEZE")]
    public string Squeeze { get; set; }
    [Column("MASH_LIFE")]
    public string MashLife { get; set; }
}