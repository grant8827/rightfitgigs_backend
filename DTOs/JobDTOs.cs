using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.DTOs
{
    public class JobRequest
    {
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
    }

    public class JobResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? ExperienceLevel { get; set; }
        public bool IsRemote { get; set; }
        public bool IsUrgentlyHiring { get; set; }
        public bool IsSeasonal { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class JobSearchRequest
    {
        public string? Search { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public string? Industry { get; set; }
        public string? ExperienceLevel { get; set; }
        public bool? IsRemote { get; set; }
        public bool? IsUrgentlyHiring { get; set; }
        public bool? IsSeasonal { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}