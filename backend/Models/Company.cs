using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Company
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Industry { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Size { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Website { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}