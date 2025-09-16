using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Model.Configurations;
using Model.Entities.Company;

namespace Model.Entities.Bookingobjects;

[Table("BOOKING_OBJECTS_BT")]
public class BookingObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("BOOKING_OBJECT_ID")]
    public int Id { get; set; }
    
    public ApplicationUser? ApplicationUser { get; set; }
    public Vineyard.Vineyard? Vineyard { get; set; }
    public Machine.Machine? Machine { get; set; }
    public Equipment? Equipment { get; set; }
    
    [JsonIgnore]
    public List<Invoice?>? Invoice { get; set; }
}