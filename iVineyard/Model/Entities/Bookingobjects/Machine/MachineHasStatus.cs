using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Bookingobjects.Machine;

[Table("MACHINE_HAS_STATUS")]
public class MachineHasStatus
{
    public Machine? Machine { get; set; }
    [Column("MACHINE_ID")]
    public int MachineId { get; set; }

    public MachineStatusType? MachineStatusType { get; set; }
    [Column("STATUS_ID")]
    public int StatusId { get; set; }
    
    [Required]
    [Column("START_DATE")]
    public DateTime? StartDate { get; set; }

    [Column("END_DATE")]
    public DateTime? EndDate{ get; set; }
}