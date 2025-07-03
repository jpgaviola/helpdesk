using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskBlazor.Models
{
    [Table("TicketSignatories")]
    public class TicketSignatory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        [StringLength(100)]
        public string SignatoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string SignatoryPosition { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation property
        [ForeignKey("TicketId")]
        public virtual Ticket? Ticket { get; set; }
    }
}