using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Configurations;

namespace Model.Entities.Bookingobjects.Vineyard;

[Table("VINEYARD_WORK_INFORMATION_JT")]
public class WorkInformation  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("WORK_INFORMATION_ID")]
    public int Id { get; set; }
    
    public ApplicationUser? ApplicationUser { get; set; }
    [Column("USER_ID")]
    public string UserId { get; set; }
    
    public Vineyard? Vineyard { get; set; }
    [Column("BOOKING_OBJECT_ID")]
    public int VineyardId { get; set; }

    public Machine.Machine? Machine { get; set; }
    [Column("MACHINE_ID")]
    public int? MachineId { get; set; }
    
    [Column("STARTED_AT")]
    public DateTime? StartedAt { get; set; }

    [Column("FINISHED_AT")]
    public DateTime? FinishedAt { get; set; }
    
    [Column("DESCRIPTION")]
    public string? Description { get; set; }
}