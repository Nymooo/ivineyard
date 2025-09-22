using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("TANK_has_WINE_BATCH")]
public class TankHasWineBatch
{
    public Tank Tank { get; set; }
    [Column("TANK_ID")]
    public int TankId { get; set; }

    public Batch Batch { get; set; }
    [Column("BATCH_ID")]
    public int BatchId { get; set; }
}