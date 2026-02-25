using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Advertisement
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "Image"; // Image, Video
        
        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FileName { get; set; }
        
        [StringLength(50)]
        public string Platform { get; set; } = "Both"; // Mobile, Web, Both

        [StringLength(30)]
        public string Placement { get; set; } = "Popup"; // Popup, PinnedFade

        [StringLength(20)]
        public string Position { get; set; } = "BottomRight"; // TopLeft, TopRight, BottomLeft, BottomRight, Center

        public int FadeDurationSeconds { get; set; } = 8;

        public bool IsDismissible { get; set; } = true;
        
        [StringLength(500)]
        public string? TargetUrl { get; set; } // Click-through URL
        
        [StringLength(100)]
        public string? BusinessName { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndDate { get; set; }
        
        public int ViewCount { get; set; } = 0;
        
        public int ClickCount { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
