using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpdeskBlazor.Models
{
    [Table("DocumentItems")]
    public class DocumentItem
    {
        public int Id { get; set; }
        public int DocumentRequestId { get; set; }

        [Required(ErrorMessage = "Document Name is required")]
        [StringLength(200, ErrorMessage = "Document Name must be less than 200 characters")]
        public string DocumentName { get; set; } = "";

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "Number of Copies is required")]
        [Range(1, 100, ErrorMessage = "Number of Copies must be between 1 and 100")]
        public int NumberOfCopies { get; set; } = 1;

        [Required(ErrorMessage = "Particulars/Reason is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Particulars must be between 10 and 2000 characters")]
        public string Particulars { get; set; } = "";

        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public DocumentRequest? DocumentRequest { get; set; }
    }
}