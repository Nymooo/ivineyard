using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Model.Entities.Company;

[Table("COMPANIES")]
public class Company
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("COMPANY_ID")]
    public int Id { get; set; }

    [Required]
    [Column("NAME")]
    [Length(0, 255)]
    public string Name { get; set; }
}