using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Application
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string JobId { get; set; } = string.Empty;
        
        [Required]
        public string WorkerId { get; set; } = string.Empty;
        
        [Required]
        public string WorkerName { get; set; } = string.Empty;
        
        [Required]
        public string WorkerEmail { get; set; } = string.Empty;
        
        public string WorkerPhone { get; set; } = string.Empty;
        
        public string WorkerSkills { get; set; } = string.Empty;
        
        public string WorkerTitle { get; set; } = string.Empty;
        
        public string WorkerLocation { get; set; } = string.Empty;
        
        public string? ResumeUrl { get; set; }
        
        [StringLength(1000)]
        public string CoverLetter { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Reviewing, Shortlisted, Rejected, Accepted
        
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Job? Job { get; set; }
        public virtual User? Worker { get; set; }
    }
}
