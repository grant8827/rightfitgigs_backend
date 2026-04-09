using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RightFitGigs.Models
{
    [Table("Job_Preferences")]
    public class JobPreference
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

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

        public bool OpenToRemote { get; set; } = true;

        [StringLength(200)]
        public string? PreferredIndustries { get; set; }

        [StringLength(50)]
        public string? EducationLevel { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual User? User { get; set; }
    }
}
