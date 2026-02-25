using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Job
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Company { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Salary { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Industry { get; set; }
        
        [StringLength(20)]
        public string? ExperienceLevel { get; set; }
        
        public bool IsRemote { get; set; } = false;
        
        public bool IsUrgentlyHiring { get; set; } = false;
        
        public bool IsSeasonal { get; set; } = false;
        
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties for future features
        public string? CompanyId { get; set; }
        public virtual Company? CompanyNavigation { get; set; }
    }
}