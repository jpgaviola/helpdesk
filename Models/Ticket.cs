using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskBlazor.Models
{
    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public int? AssignedToUserId { get; set; }

        [StringLength(1000)]
        public string? Resolution { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public bool IsDraft { get; set; } = false;

        [StringLength(50)]
        public string? AppReferenceNo { get; set; }

        public DateTime? RequestDate { get; set; }

        [StringLength(100)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public DateTime? DateNeeded { get; set; }

        public TimeOnly? TimeNeeded { get; set; }

        // Navigation properties
        [ForeignKey("AssignedToUserId")]
        public virtual User? AssignedToUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("ModifiedBy")]
        public virtual User? ModifiedByUser { get; set; }

        // Related entities
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
        public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
        public virtual ICollection<TicketSignatory> Signatories { get; set; } = new List<TicketSignatory>();
    }
}