using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Entities.Harvest;

[Table("TANK_MOVEMENT")]
public class TankMovement
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("MOVEMENT_ID")]
    public int MovementId { get; set; }

    [Column("FROM_TANK")]
    public int? FromTakId { get; set; }              // << nullable, kein 0er-Placeholder
    [JsonIgnore] 
    public Tank? FromTank { get; set; }

    [Column("TO_TANK")]
    public int ToTankId { get; set; }                // required
    [JsonIgnore] 
    public Tank? ToTank { get; set; }

    [Column("DATE")]
    public DateTime Date { get; set; }

    [Column("VOLUME")]
    public double Volume { get; set; }

    [Column("BATCH_ID")]
    public int BatchId { get; set; }                 // required FK
    [JsonIgnore] 
    public Batch? Batch { get; set; }
}