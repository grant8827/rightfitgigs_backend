using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.DTOs
{
    public class UserRequest
    {
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
        
        [Required]
        [StringLength(10)]
        public string UserType { get; set; } = "Worker";
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        // Company fields (for Employer registration)
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? CompanySize { get; set; }
        
        [StringLength(100)]
        public string? Industry { get; set; }
        
        [StringLength(200)]
        public string? Website { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }
    
    public class RegisterRequest
    {
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
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Location { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [StringLength(500)]
        public string? Skills { get; set; }
        
        [StringLength(100)]
        public string? Title { get; set; }
        
        [Required]
        [StringLength(10)]
        public string UserType { get; set; } = "Worker";
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        // Company fields (for Employer registration)
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? CompanySize { get; set; }
        
        [StringLength(100)]
        public string? Industry { get; set; }
        
        [StringLength(200)]
        public string? Website { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        
        // Worker-specific fields
        public string? ResumeUrl { get; set; }
        public string? DesiredJobTitle { get; set; }
        public string? DesiredLocation { get; set; }
        public string? DesiredSalaryRange { get; set; }
        public string? DesiredJobType { get; set; }
        public string? DesiredExperienceLevel { get; set; }
        public bool OpenToRemote { get; set; }
        public string? PreferredIndustries { get; set; }
    }

    public class UpdateProfileRequest
    {
        [StringLength(50)]
        public string? FirstName { get; set; }
        
        [StringLength(50)]
        public string? LastName { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Location { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [StringLength(500)]
        public string? Skills { get; set; }
        
        [StringLength(100)]
        public string? Title { get; set; }
        
        // Job preferences
        [StringLength(100)]
        public string? DesiredJobTitle { get; set; }
        
        [StringLength(200)]
        public string? DesiredLocation { get; set; }
        
        [StringLength(50)]
        public string? DesiredSalaryRange { get; set; }
        
        [StringLength(50)]
        public string? DesiredJobType { get; set; }
        
        [StringLength(50)]
        public string? DesiredExperienceLevel { get; set; }
        
        public bool? OpenToRemote { get; set; }
        
        [StringLength(200)]
        public string? PreferredIndustries { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}