using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskBlazor.Models
{
    [Table("DocumentRequests")]
    public class DocumentRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required]
        public DateTime DateNeeded { get; set; }

        [Required]
        public TimeOnly TimeNeeded { get; set; }

        [Required]
        [StringLength(2000)]
        public string Particulars { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }

        public virtual ICollection<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
        public virtual ICollection<DocumentRequestAttachment> Attachments { get; set; } = new List<DocumentRequestAttachment>();
    }
}