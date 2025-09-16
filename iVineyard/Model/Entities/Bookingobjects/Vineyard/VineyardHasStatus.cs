using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Model.Entities.Bookingobjects.Vineyard;

[Table("VINEYARD_HAS_STATUS_JT")]
public class VineyardHasStatus
{
    public Vineyard? Vineyard { get; set; }
    [Column("BOOKING_OBJECT_ID")]
    public int VineyardId { get; set; }

    public VineyardStatusType? VineyardStatusType { get; set; }
    [Column("STATUS_ID")]
    public int StatusId { get; set; }
    
    [Required]
    [Column("START_DATE")]
    public DateTime StartDate { get; set; }

    [Column("END_DATE")]
    public DateTime? EndDate { get; set; }
}