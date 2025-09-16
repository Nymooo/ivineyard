using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities.Bookingobjects;

namespace Model.Entities.Company;

[Table("INVOICES")]
public class Invoice
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("INVOICE_ID")]
    public int Id { get; set; }

    public BookingObject? BookingObject { get; set; } = new();
    [Column("BOOKING_OBJECT_ID")]
    public int? BookingObjectId { get; set; }
    
    [Required]
    [Column("PRICE")]
    public double Price { get; set; }

    [Required]
    [Column("BOUGHT_AT")]
    public DateTime? BoughAt { get; set; }
    
    [Required]
    [Column("DESCRIPTION")]
    public string Description { get; set; }
}