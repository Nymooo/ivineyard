using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("WHITE_WINE_RED_WINE")]
public class WhiteWine_RedWine : Informations
{
    [Column("ALCOHOL")]
    public double Alcohol { get; set; }
    [Column("RESIDUAL_SUGAR")]
    public double ResidualSugar { get; set; }
    [Column("SULFUR")]
    public double Sulfur { get; set; }
}