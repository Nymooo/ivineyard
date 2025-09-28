using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Entities.Harvest;
[Table("YOUNG_WINE")]
public class YoungWine
{
    [JsonIgnore]
    public Informations? Informations { get; set; }
    [Column("INFORMATION_ID"), Required]
    
    public int Id { get; set; }
    [Column("ALCOHOL")]
    public double Alcohol { get; set; }
    [Column("RESIDUAL_SUGAR")]
    public string ResidualSugar { get; set; }
}