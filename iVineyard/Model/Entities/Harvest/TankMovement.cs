using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("TANK_MOVEMENT")]
public class TankMovement
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("MOVEMENT_ID")]
    public int MovementId { get; set; }
    public Tank FromTank { get; set; }
    [Column("FROM_TANK")]
    public int FromTakId { get; set; }

    public Tank ToTank { get; set; }
    [Column("TO_TANK")]
    public int ToTankId { get; set; }

    [Column("DATE")]
    public DateTime Date { get; set; }
    [Column("VOLUME")]
    public double Volume { get; set; }
}