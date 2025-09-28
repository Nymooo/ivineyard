using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities.Harvest;
[Table("TREATMENT")]
public class Treatment
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("TREATMENT_ID")]
    public int TreatmentId { get; set; }
    [Column("TYPE")]
    public string Type { get; set; }
    
    [Column("NOTES")]
    public string Notes { get; set; } = string.Empty;
}