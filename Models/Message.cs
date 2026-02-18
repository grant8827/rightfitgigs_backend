using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Models
{
    public class Message
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string SenderId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string SenderName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string SenderType { get; set; } = string.Empty; // "Employer" or "Worker"
        
        [Required]
        [StringLength(100)]
        public string ReceiverId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string ReceiverType { get; set; } = string.Empty; // "Employer" or "Worker"
        
        [StringLength(200)]
        public string? Subject { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadDate { get; set; }
        
        // Optional job reference if message is about a specific job
        public string? JobId { get; set; }
        public virtual Job? Job { get; set; }
        
        // Conversation thread ID to group related messages
        [StringLength(100)]
        public string ConversationId { get; set; } = string.Empty;
    }
}