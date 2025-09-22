using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("TANK")]
public class Tank
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("TANK_ID")]
    public int TankId { get; set; }
}