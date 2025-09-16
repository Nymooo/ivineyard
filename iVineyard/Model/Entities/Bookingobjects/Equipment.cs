using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model.Entities.Bookingobjects;

[Table("EQUIPMENTS")]
public class Equipment
{
    [Key]
    [Column("BOOKING_OBJECT_ID")]
    public int BookingObjectId { get; set; }
    [JsonIgnore]
    public BookingObject? BookingObject { get; set; }
    
    [Required]
    [Column("NAME"), Length(0, 100)]
    public string Name { get; set; }
}