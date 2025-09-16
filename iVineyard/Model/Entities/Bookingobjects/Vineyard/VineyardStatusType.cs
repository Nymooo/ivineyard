using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Bookingobjects.Vineyard;

[Table("E_VINEYARD_STATUS_TYPE")]
public class VineyardStatusType
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("STATUS_ID")]
    public int Id { get; set; }

    [Required]
    [Column("TYPE"), Length(0, 30)]
    public string Type { get; set; }

    public List<VineyardHasStatus>? StatusList { get; set; } = new();
}