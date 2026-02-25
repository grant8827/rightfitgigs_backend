using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class AppMetric
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string MetricType { get; set; } = string.Empty; // Visit, Download

        [StringLength(30)]
        public string Platform { get; set; } = string.Empty; // Web, Android, Apple

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
