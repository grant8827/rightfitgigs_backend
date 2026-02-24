using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.DTOs
{
    public class ApplicationRequest
    {
        [Required]
        public string JobId { get; set; } = string.Empty;
        
        [Required]
        public string WorkerId { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? CoverLetter { get; set; }
    }

    public class ApplicationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string JobId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string WorkerId { get; set; } = string.Empty;
        public string WorkerName { get; set; } = string.Empty;
        public string WorkerEmail { get; set; } = string.Empty;
        public string WorkerPhone { get; set; } = string.Empty;
        public string WorkerSkills { get; set; } = string.Empty;
        public string WorkerTitle { get; set; } = string.Empty;
        public string WorkerLocation { get; set; } = string.Empty;
        public string? ResumeUrl { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class UpdateApplicationStatusRequest
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
