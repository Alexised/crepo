
using CargaAmbulatoria.EntityFramework.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargaAmbulatoria.EntityFramework.Models
{
    public class Document : IDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
        public DateTime DocumentDate { get; set; }

        public string UserId { get; set; }
        public long CohortId { get; set; }
        public string Dni { get; set; }
        public string Regime { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(CohortId))]
        public virtual Cohort? Cohort { get; set; }

    }
}
