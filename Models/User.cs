using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Bio { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Skills { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string UserType { get; set; } = "Worker"; // Worker, Employer
        
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        // Worker-specific fields
        [StringLength(500)]
        public string? ResumeUrl { get; set; }
        
        [StringLength(100)]
        public string? DesiredJobTitle { get; set; }
        
        [StringLength(200)]
        public string? DesiredLocation { get; set; }
        
        [StringLength(50)]
        public string? DesiredSalaryRange { get; set; }
        
        [StringLength(50)]
        public string? DesiredJobType { get; set; } // Full-time, Part-time, Contract, Freelance
        
        [StringLength(50)]
        public string? DesiredExperienceLevel { get; set; } // Entry, Mid, Senior
        
        public bool OpenToRemote { get; set; } = true;
        
        [StringLength(200)]
        public string? PreferredIndustries { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        public bool IsAdmin { get; set; } = false;
        
        // Navigation properties
        public string? CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        
        public string Initials => $"{FirstName.FirstOrDefault()}.{LastName.FirstOrDefault()}".ToUpper();
    }
}