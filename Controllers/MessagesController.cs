using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponse>> SendMessage([FromBody] MessageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Generate conversation ID if not provided
                var conversationId = request.ConversationId;
                if (string.IsNullOrEmpty(conversationId))
                {
                    // Create a consistent conversation ID between two users
                    var participants = new[] { request.SenderId, request.ReceiverId }.OrderBy(x => x).ToArray();
                    conversationId = $"{participants[0]}_{participants[1]}";
                    if (!string.IsNullOrEmpty(request.JobId))
                    {
                        conversationId += $"_job_{request.JobId}";
                    }
                }

                var message = new Message
                {
                    SenderId = request.SenderId,
                    SenderName = request.SenderName,
                    SenderType = request.SenderType,
                    ReceiverId = request.ReceiverId,
                    ReceiverName = request.ReceiverName,
                    ReceiverType = request.ReceiverType,
                    Subject = request.Subject,
                    Content = request.Content,
                    JobId = request.JobId,
                    ConversationId = conversationId
                };

                _context.Messages.Add(message);

                // Create notification for the recipient
                var notificationMessage = $"{request.SenderName} sent you a message";
                string? jobTitle = null;
                
                if (!string.IsNullOrEmpty(request.JobId))
                {
                    var job = await _context.Jobs.FindAsync(request.JobId);
                    if (job != null)
                    {
                        jobTitle = job.Title;
                        notificationMessage = $"{request.SenderName} sent you a message about {job.Title}";
                    }
                }

                var notification = new Notification
                {
                    UserId = request.ReceiverId,
                    Type = "NewMessage",
                    Title = "You have a new message",
                    Message = notificationMessage,
                    RelatedId = message.Id,
                    JobId = request.JobId,
                    JobTitle = jobTitle
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var response = new MessageResponse
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = message.SenderName,
                    SenderType = message.SenderType,
                    ReceiverId = message.ReceiverId,
                    ReceiverName = message.ReceiverName,
                    ReceiverType = message.ReceiverType,
                    Subject = message.Subject,
                    Content = message.Content,
                    IsRead = message.IsRead,
                    SentDate = message.SentDate,
                    ReadDate = message.ReadDate,
                    JobId = message.JobId,
                    ConversationId = message.ConversationId
                };

                return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageResponse>> GetMessage(string id)
        {
            try
            {
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
                
                if (message == null)
                {
                    return NotFound($"Message with ID {id} not found");
                }

                var response = new MessageResponse
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    SenderName = message.SenderName,
                    SenderType = message.SenderType,
                    ReceiverId = message.ReceiverId,
                    ReceiverName = message.ReceiverName,
                    ReceiverType = message.ReceiverType,
                    Subject = message.Subject,
                    Content = message.Content,
                    IsRead = message.IsRead,
                    SentDate = message.SentDate,
                    ReadDate = message.ReadDate,
                    JobId = message.JobId,
                    ConversationId = message.ConversationId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetConversationMessages(string conversationId, [FromQuery] MessageSearchRequest request)
        {
            try
            {
                var query = _context.Messages
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.SentDate);

                var totalCount = await query.CountAsync();
                var messages = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var response = messages.Select(m => new MessageResponse
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.SenderName,
                    SenderType = m.SenderType,
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.ReceiverName,
                    ReceiverType = m.ReceiverType,
                    Subject = m.Subject,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    SentDate = m.SentDate,
                    ReadDate = m.ReadDate,
                    JobId = m.JobId,
                    ConversationId = m.ConversationId
                }).ToList();

                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<IEnumerable<ConversationResponse>>> GetUserConversations(string userId)
        {
            try
            {
                var conversations = await _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .GroupBy(m => m.ConversationId)
                    .Select(g => new
                    {
                        ConversationId = g.Key,
                        LastMessage = g.OrderByDescending(m => m.SentDate).First(),
                        UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
                    })
                    .ToListAsync();

                var response = new List<ConversationResponse>();
                
                foreach (var c in conversations)
                {
                    string? jobTitle = null;
                    if (!string.IsNullOrEmpty(c.LastMessage.JobId))
                    {
                        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == c.LastMessage.JobId);
                        jobTitle = job?.Title;
                    }

                    response.Add(new ConversationResponse
                    {
                        ConversationId = c.ConversationId,
                        OtherParticipantId = c.LastMessage.SenderId == userId ? c.LastMessage.ReceiverId : c.LastMessage.SenderId,
                        OtherParticipantName = c.LastMessage.SenderId == userId ? c.LastMessage.ReceiverName : c.LastMessage.SenderName,
                        OtherParticipantType = c.LastMessage.SenderId == userId ? c.LastMessage.ReceiverType : c.LastMessage.SenderType,
                        JobId = c.LastMessage.JobId,
                        JobTitle = jobTitle,
                        LastMessageContent = c.LastMessage.Content,
                        LastMessageDate = c.LastMessage.SentDate,
                        HasUnreadMessages = c.UnreadCount > 0,
                        UnreadCount = c.UnreadCount
                    });
                }

                return Ok(response.OrderByDescending(c => c.LastMessageDate).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkMessageAsRead(string id)
        {
            try
            {
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
                
                if (message == null)
                {
                    return NotFound($"Message with ID {id} not found");
                }

                if (!message.IsRead)
                {
                    message.IsRead = true;
                    message.ReadDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("conversation/{conversationId}/mark-read/{userId}")]
        public async Task<IActionResult> MarkConversationAsRead(string conversationId, string userId)
        {
            try
            {
                var unreadMessages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId && m.ReceiverId == userId && !m.IsRead)
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                    message.ReadDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            try
            {
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
                
                if (message == null)
                {
                    return NotFound($"Message with ID {id} not found");
                }

                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}