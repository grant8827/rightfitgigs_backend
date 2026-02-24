using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.DTOs
{
    public class MessageRequest
    {
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
        
        public string? JobId { get; set; }
        
        public string? ConversationId { get; set; }
    }

    public class MessageResponse
    {
        public string Id { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderType { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public string? JobId { get; set; }
        public string ConversationId { get; set; } = string.Empty;
    }

    public class ConversationResponse
    {
        public string ConversationId { get; set; } = string.Empty;
        public string OtherParticipantId { get; set; } = string.Empty;
        public string OtherParticipantName { get; set; } = string.Empty;
        public string OtherParticipantType { get; set; } = string.Empty;
        public string? JobId { get; set; }
        public string? JobTitle { get; set; }
        public string LastMessageContent { get; set; } = string.Empty;
        public DateTime LastMessageDate { get; set; }
        public bool HasUnreadMessages { get; set; }
        public int UnreadCount { get; set; }
    }

    public class MessageSearchRequest
    {
        public string? UserId { get; set; }
        public string? ConversationId { get; set; }
        public bool? IsRead { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}