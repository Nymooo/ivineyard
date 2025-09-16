using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Bookingobjects.Machine;

[Table("E_MACHINE_STATUS_TYPES")]
public class MachineStatusType
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("STATUS_ID")]
    public int Id { get; set; }

    [Column("TYPE"), Length(0, 45)]
    public string Type { get; set; }
    
   
}