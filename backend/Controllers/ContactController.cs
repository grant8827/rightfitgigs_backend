using Microsoft.AspNetCore.Mvc;
using RightFitGigs.Services;
using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<ContactController> _logger;
        private readonly IConfiguration _configuration;

        public ContactController(EmailService emailService, ILogger<ContactController> logger, IConfiguration configuration)
        {
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        // POST /api/contact  — public, no auth required
        [HttpPost]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Please fill in all required fields correctly." });

            // Basic honeypot check (bot trap — bots fill hidden fields)
            if (!string.IsNullOrEmpty(request.Website))
                return Ok(new { message = "Message sent." }); // silently discard

            try
            {
                var adminEmail = _configuration["AdminEmail"] ?? _configuration["Mailtrap:FromEmail"] ?? "info@rightfitgigs.com";
                var adminName  = "RightFitGigs Admin";

                var html = $@"
                    <div style=""font-family:sans-serif;max-width:600px;margin:auto;"">
                        <div style=""background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:24px 32px;border-radius:12px 12px 0 0;"">
                            <h2 style=""color:white;margin:0;"">New Contact Form Message</h2>
                        </div>
                        <div style=""background:#f8f9fa;padding:32px;border-radius:0 0 12px 12px;border:1px solid #e5e7eb;"">
                            <table style=""width:100%;border-collapse:collapse;"">
                                <tr><td style=""padding:8px 0;color:#6b7280;width:100px;""><strong>Name</strong></td><td style=""padding:8px 0;color:#111827;"">{System.Net.WebUtility.HtmlEncode(request.Name)}</td></tr>
                                <tr><td style=""padding:8px 0;color:#6b7280;""><strong>Email</strong></td><td style=""padding:8px 0;""><a href=""mailto:{System.Net.WebUtility.HtmlEncode(request.Email)}"" style=""color:#4f46e5;"">{System.Net.WebUtility.HtmlEncode(request.Email)}</a></td></tr>
                                <tr><td style=""padding:8px 0;color:#6b7280;""><strong>Subject</strong></td><td style=""padding:8px 0;color:#111827;"">{System.Net.WebUtility.HtmlEncode(request.Subject)}</td></tr>
                            </table>
                            <hr style=""border:none;border-top:1px solid #e5e7eb;margin:20px 0;"" />
                            <p style=""color:#6b7280;margin:0 0 8px;""><strong>Message</strong></p>
                            <p style=""color:#111827;white-space:pre-wrap;background:white;padding:16px;border-radius:8px;border:1px solid #e5e7eb;margin:0;"">{System.Net.WebUtility.HtmlEncode(request.Message)}</p>
                            <hr style=""border:none;border-top:1px solid #e5e7eb;margin:20px 0;"" />
                            <p style=""color:#9ca3af;font-size:0.85rem;margin:0;"">Sent via RightFitGigs contact form</p>
                        </div>
                    </div>";

                await _emailService.SendAsync(adminEmail, adminName,
                    $"[Contact Form] {request.Subject} — from {request.Name}",
                    html);

                _logger.LogInformation("Contact form submitted by {Email} — subject: {Subject}", request.Email, request.Subject);

                return Ok(new { message = "Your message has been sent. We'll get back to you within 24 hours." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process contact form from {Email}", request.Email);
                return StatusCode(500, new { error = "Unable to send your message right now. Please try again later." });
            }
        }
    }

    public class ContactRequest
    {
        [Required, MinLength(2), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        // Honeypot — must be empty
        public string? Website { get; set; }
    }
}
