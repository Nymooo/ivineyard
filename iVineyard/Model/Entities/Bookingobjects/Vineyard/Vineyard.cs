using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Model.Entities.Bookingobjects.Vineyard;

[Table("VINEYARDS")]
public class Vineyard
{
    [Key]
    [Column("BOOKING_OBJECT_ID")]
    public int BookingObjectId { get; set; }
    [JsonIgnore]
    public BookingObject? BookingObject { get; set; }
    
    public Company.Company? Company { get; set; }
    [Column("COMPANY_ID")]
    public int? CompanyId { get; set; }

    [Required]
    [Column("NAME"), Length(0, 100)]
    public string Name { get; set; }

    [Required]
    [Column("COORDINATES")]
    public string Coordinates { get; set; }

    [Required]
    [Column("MID_COORDINATE"), Length(0, 255)]
    public string MidCoordinate { get; set; }
    
    [Required]
    [Column("AREA")]
    public float Area { get; set; }
    
    public List<VineyardHasStatus> StatusList { get; set; } = new();
}