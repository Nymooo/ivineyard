using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Entities.Harvest;
[Table("WHITE_WINE_RED_WINE")]
public class WhiteWine_RedWine
{
    [JsonIgnore]
    public Informations? Informations { get; set; }
    [Column("INFORMATION_ID"), Required]
    public int Id { get; set; }
    
    [Column("ALCOHOL")]
    public double Alcohol { get; set; }
    [Column("RESIDUAL_SUGAR")]
    public double ResidualSugar { get; set; }
    [Column("SULFUR")]
    public double Sulfur { get; set; }
}