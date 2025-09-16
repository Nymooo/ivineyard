using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Model.Entities.Bookingobjects;
using Model.Entities.Company;

namespace Model.Configurations;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        
    }
    
    [Column("BOOKING_OBJECT_ID"), Required]
    public int BookingObjectId { get; set; }
    [JsonIgnore]
    public BookingObject? BookingObject { get; set; }

    [Column("COMPANY_ID")]
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    [Column("SALARY")]
    public double Salary { get; set; }
}