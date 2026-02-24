using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // "NewMessage", "NewApplication", etc.
        
        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Message { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadDate { get; set; }
        
        // Optional references
        public string? RelatedId { get; set; } // Message ID, Application ID, etc.
        
        public string? JobId { get; set; }
        
        public string? JobTitle { get; set; }
    }
}
