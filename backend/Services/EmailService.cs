using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RightFitGigs.Services
{
    public class MailtrapSettings
    {
        public string ApiToken { get; set; } = string.Empty;
        public string FromEmail { get; set; } = "info@rightfitgigs.com";
        public string FromName { get; set; } = "RightFitGigs";
    }

    public class EmailService
    {
        private readonly MailtrapSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string SendUrl = "https://send.api.mailtrap.io/api/send";

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _settings = configuration.GetSection("Mailtrap").Get<MailtrapSettings>() ?? new MailtrapSettings();
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // ─── Test Send (throws on error so the test endpoint surfaces details) ──

        public async Task<object> SendTestAsync(string toEmail)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiToken))
                throw new InvalidOperationException("Mailtrap:ApiToken is not configured.");

            await SendCoreAsync(toEmail, "Test", "✅ RightFitGigs Email Test",
                "<div style='font-family:sans-serif;max-width:600px;margin:auto;'>" +
                "<div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;border-radius:12px 12px 0 0;'>" +
                "<h1 style='color:white;margin:0;'>Email Test ✅</h1></div>" +
                "<div style='padding:2rem;background:#f9fafb;border-radius:0 0 12px 12px;'>" +
                "<p>Mailtrap API is working correctly for <strong>RightFitGigs</strong>.</p></div></div>");

            return new { success = true, message = $"Email sent to {toEmail}", from = _settings.FromEmail };
        }

        // ─── Core Send ────────────────────────────────────────────────────────

        public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiToken))
            {
                _logger.LogWarning("Email not sent — Mailtrap:ApiToken is not configured. Subject: {Subject}", subject);
                return;
            }

            try
            {
                await SendCoreAsync(toEmail, toName, subject, htmlBody);
                _logger.LogInformation("Email sent to {Email} — {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} — {Subject}", toEmail, subject);
                // Don't throw — email failure should never break the main request
            }
        }

        private async Task SendCoreAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var payload = new
            {
                from = new { email = _settings.FromEmail, name = _settings.FromName },
                to = new[] { new { email = toEmail, name = toName } },
                subject,
                html = htmlBody
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.ApiToken);

            var response = await client.PostAsync(SendUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Mailtrap API returned {(int)response.StatusCode}: {body}");
            }
        }

        // ─── Welcome Email ────────────────────────────────────────────────────

        public async Task SendWelcomeAsync(string email, string firstName, string userType)
        {
            var role = userType == "Employer" ? "employer" : "job seeker";
            var html = $@"
<div style='font-family:sans-serif;max-width:600px;margin:auto;background:#f9fafb;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;'>
    <h1 style='color:white;margin:0;font-size:1.8rem;'>Welcome to RightFitGigs!</h1>
  </div>
  <div style='padding:2rem;'>
    <p style='font-size:1.1rem;color:#374151;'>Hi <strong>{firstName}</strong>,</p>
    <p style='color:#6b7280;line-height:1.7;'>
      Your account has been created successfully as a <strong>{role}</strong>.
      {(userType == "Employer"
        ? "You can now post jobs and review applications from talented workers."
        : "You can now browse jobs and apply with your profile.")}
    </p>
    <div style='text-align:center;margin:2rem 0;'>
      <a href='https://rightfitgigs.com' style='background:linear-gradient(135deg,#4f46e5,#14b8a6);color:white;padding:0.85rem 2rem;border-radius:8px;text-decoration:none;font-weight:600;font-size:1rem;'>
        Get Started
      </a>
    </div>
    <p style='color:#9ca3af;font-size:0.85rem;text-align:center;margin-top:2rem;'>
      RightFitGigs &mdash; Connecting talent with opportunity
    </p>
  </div>
</div>";

            await SendAsync(email, firstName, "Welcome to RightFitGigs! 🎉", html);
        }

        // ─── Application Submitted (to worker) ───────────────────────────────

        public async Task SendApplicationConfirmationAsync(string workerEmail, string workerName, string jobTitle, string company)
        {
            var html = $@"
<div style='font-family:sans-serif;max-width:600px;margin:auto;background:#f9fafb;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;'>
    <h1 style='color:white;margin:0;font-size:1.6rem;'>Application Submitted ✅</h1>
  </div>
  <div style='padding:2rem;'>
    <p style='font-size:1.05rem;color:#374151;'>Hi <strong>{workerName}</strong>,</p>
    <p style='color:#6b7280;line-height:1.7;'>
      Your application for <strong>{jobTitle}</strong> at <strong>{company}</strong> has been submitted successfully.
    </p>
    <div style='background:#eef2ff;border-left:4px solid #4f46e5;border-radius:6px;padding:1rem 1.25rem;margin:1.5rem 0;'>
      <p style='margin:0;color:#3730a3;font-weight:500;'>What happens next?</p>
      <p style='margin:0.5rem 0 0;color:#6b7280;font-size:0.9rem;line-height:1.6;'>
        The employer will review your profile and cover letter. You'll receive a notification if your status changes.
        You can track your applications in your dashboard.
      </p>
    </div>
    <p style='color:#9ca3af;font-size:0.85rem;text-align:center;margin-top:2rem;'>
      RightFitGigs &mdash; Good luck with your application!
    </p>
  </div>
</div>";

            await SendAsync(workerEmail, workerName, $"Application submitted: {jobTitle} at {company}", html);
        }

        // ─── New Application Alert (to employer) ─────────────────────────────

        public async Task SendNewApplicationAlertAsync(string employerEmail, string employerName, string workerName, string jobTitle, string applicationId)
        {
            var html = $@"
<div style='font-family:sans-serif;max-width:600px;margin:auto;background:#f9fafb;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;'>
    <h1 style='color:white;margin:0;font-size:1.6rem;'>New Application Received 📬</h1>
  </div>
  <div style='padding:2rem;'>
    <p style='font-size:1.05rem;color:#374151;'>Hi <strong>{employerName}</strong>,</p>
    <p style='color:#6b7280;line-height:1.7;'>
      <strong>{workerName}</strong> has applied for your <strong>{jobTitle}</strong> position.
    </p>
    <div style='text-align:center;margin:2rem 0;'>
      <a href='https://rightfitgigs.com' style='background:linear-gradient(135deg,#4f46e5,#14b8a6);color:white;padding:0.85rem 2rem;border-radius:8px;text-decoration:none;font-weight:600;font-size:1rem;'>
        Review Application
      </a>
    </div>
    <p style='color:#9ca3af;font-size:0.85rem;text-align:center;margin-top:2rem;'>
      RightFitGigs &mdash; Connecting talent with opportunity
    </p>
  </div>
</div>";

            await SendAsync(employerEmail, employerName, $"New application for {jobTitle} — {workerName}", html);
        }

        // ─── Application Status Update (to worker) ───────────────────────────

        public async Task SendStatusUpdateAsync(string workerEmail, string workerName, string jobTitle, string company, string newStatus)
        {
            var (emoji, headline, message) = newStatus.ToLower() switch
            {
                "accepted"    => ("🎉", "Great news!", $"Your application for <strong>{jobTitle}</strong> at <strong>{company}</strong> has been <strong style='color:#14b8a6;'>accepted</strong>. Congratulations!"),
                "interviewing"=> ("📅", "Interview Invitation!", $"You've been invited to interview for <strong>{jobTitle}</strong> at <strong>{company}</strong>. Log in to your dashboard for details."),
                "offer"       => ("💼", "Job Offer!", $"You've received a job offer for <strong>{jobTitle}</strong> at <strong>{company}</strong>. Log in to review it."),
                "rejected"    => ("📋", "Application Update", $"Thank you for applying for <strong>{jobTitle}</strong> at <strong>{company}</strong>. Unfortunately, you were not selected at this time. Don't give up — more opportunities await!"),
                "reviewing"   => ("👀", "Application Being Reviewed", $"Your application for <strong>{jobTitle}</strong> at <strong>{company}</strong> is currently being reviewed."),
                _             => ("📢", "Application Update", $"Your application status for <strong>{jobTitle}</strong> at <strong>{company}</strong> has been updated to <strong>{newStatus}</strong>."),
            };

            var html = $@"
<div style='font-family:sans-serif;max-width:600px;margin:auto;background:#f9fafb;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;'>
    <div style='font-size:2.5rem;margin-bottom:0.5rem;'>{emoji}</div>
    <h1 style='color:white;margin:0;font-size:1.6rem;'>{headline}</h1>
  </div>
  <div style='padding:2rem;'>
    <p style='font-size:1.05rem;color:#374151;'>Hi <strong>{workerName}</strong>,</p>
    <p style='color:#6b7280;line-height:1.7;'>{message}</p>
    <div style='text-align:center;margin:2rem 0;'>
      <a href='https://rightfitgigs.com' style='background:linear-gradient(135deg,#4f46e5,#14b8a6);color:white;padding:0.85rem 2rem;border-radius:8px;text-decoration:none;font-weight:600;font-size:1rem;'>
        View Dashboard
      </a>
    </div>
    <p style='color:#9ca3af;font-size:0.85rem;text-align:center;margin-top:2rem;'>
      RightFitGigs &mdash; Connecting talent with opportunity
    </p>
  </div>
</div>";

            await SendAsync(workerEmail, workerName, $"{emoji} Application update: {jobTitle} at {company}", html);
        }

        // ─── Password Reset ──────────────────────────────────────────────────

        public async Task SendPasswordResetAsync(string toEmail, string firstName, string resetLink)
        {
            var html = $@"
<div style='font-family:sans-serif;max-width:600px;margin:auto;background:#f9fafb;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(135deg,#4f46e5,#14b8a6);padding:2rem;text-align:center;'>
    <div style='font-size:2.5rem;margin-bottom:0.5rem;'>🔑</div>
    <h1 style='color:white;margin:0;font-size:1.6rem;'>Reset Your Password</h1>
  </div>
  <div style='padding:2rem;'>
    <p style='font-size:1.05rem;color:#374151;'>Hi <strong>{firstName}</strong>,</p>
    <p style='color:#6b7280;line-height:1.7;'>
      We received a request to reset your password. Click the button below to choose a new one.
      This link expires in <strong>1 hour</strong>.
    </p>
    <div style='text-align:center;margin:2rem 0;'>
      <a href='{resetLink}' style='background:linear-gradient(135deg,#4f46e5,#14b8a6);color:white;padding:0.85rem 2rem;border-radius:8px;text-decoration:none;font-weight:600;font-size:1rem;'>
        Reset Password
      </a>
    </div>
    <p style='color:#6b7280;font-size:0.9rem;line-height:1.6;'>
      If you didn't request a password reset, you can safely ignore this email — your password won't change.
    </p>
    <p style='color:#9ca3af;font-size:0.85rem;text-align:center;margin-top:2rem;'>
      RightFitGigs &mdash; Connecting talent with opportunity
    </p>
  </div>
</div>";

            await SendAsync(toEmail, firstName, "Reset your RightFitGigs password", html);
        }
    }
}
