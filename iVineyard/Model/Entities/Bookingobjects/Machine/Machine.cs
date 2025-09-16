using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Model.Entities.Bookingobjects.Vineyard;

namespace Model.Entities.Bookingobjects.Machine;

[Table("MACHINES")]
public class Machine
{
    [Key]
    [Column("BOOKING_OBJECT_ID")]
    public int BookingObjectId { get; set; }
    [JsonIgnore]
    public BookingObject? BookingObject { get; set; }
    public List<MachineHasStatus> MachineHasStatusList { get; set; } = new();
    [Required]
    [Column("NAME")]
    public string Name { get; set; }

    [JsonIgnore]
    public List<WorkInformation> WorkInformation { get; set; } = new();
}