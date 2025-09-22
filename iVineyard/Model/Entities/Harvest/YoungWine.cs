using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("YOUNG_WINE")]
public class YoungWine : Informations
{
    [Column("ALCOHOL")]
    public double Alcohol { get; set; }
    [Column("RESIDUAL_SUGAR")]
    public double ResidualSugar { get; set; }
}