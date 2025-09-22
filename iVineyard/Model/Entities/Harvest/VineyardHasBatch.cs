using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities.Bookingobjects.Vineyard;

namespace Model.Entities.Harvest;

[Table("VINEYARD_HAS_BATCH")]
public class VineyardHasBatch
{
    public Vineyard Vineyard { get; set; }
    [Column("VINEYARD_ID")]
    public int VineyardId { get; set; }

    public Batch Batch { get; set; }
    [Column("BATCH_ID")]
    public int BatchId { get; set; }
}