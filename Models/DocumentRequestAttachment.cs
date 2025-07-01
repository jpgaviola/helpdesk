using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskBlazor.Models
{
    [Table("DocumentRequestAttachments")]
    public class DocumentRequestAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DocumentRequestId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        public DateTime UploadedDate { get; set; } = DateTime.Now;

        public int? UploadedBy { get; set; }

        // Navigation properties
        [ForeignKey("DocumentRequestId")]
        public virtual DocumentRequest DocumentRequest { get; set; } = null!;

        [ForeignKey("UploadedBy")]
        public virtual User? UploadedByUser { get; set; }
    }
}