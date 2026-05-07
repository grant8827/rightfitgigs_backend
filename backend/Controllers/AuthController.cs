using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;
using RightFitGigs.Services;
using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;
        private readonly PendingRegistrationStore _pendingStore;
        private readonly string _frontendBaseUrl;

        public AuthController(ApplicationDbContext context, IWebHostEnvironment environment, EmailService emailService, JwtService jwtService, PendingRegistrationStore pendingStore, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
            _jwtService = jwtService;
            _pendingStore = pendingStore;

            var configuredFrontendUrl = configuration["FRONTEND_URL"];
            _frontendBaseUrl = NormalizeFrontendUrl(configuredFrontendUrl);
        }

        // ─── Step 1: Validate data, store pending, send OTP ─────────────────
        [HttpPost("register/initiate")]
        public async Task<IActionResult> RegisterInitiate([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var normalizedEmail = request.Email.Trim().ToLowerInvariant();
                var normalizedUserType = NormalizeUserType(request.UserType);

                if (normalizedUserType != "Worker" && normalizedUserType != "Employer")
                    return BadRequest("User type must be Worker or Employer.");

                if (normalizedUserType == "Employer" && string.IsNullOrWhiteSpace(request.CompanyName))
                    return BadRequest("Company name is required for employer registration.");

                // Check the real DB — don't allow duplicate emails
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
                if (existingUser != null)
                    return Conflict("An account with this email already exists.");

                // Build pending record (password hashed here so we never store plaintext)
                var otp = PendingRegistrationStore.GenerateOtp();
                var pending = new PendingRegistration
                {
                    Email        = normalizedEmail,
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName    = request.FirstName,
                    LastName     = request.LastName,
                    Phone        = request.Phone ?? string.Empty,
                    Location     = request.Location ?? string.Empty,
                    Bio          = request.Bio ?? string.Empty,
                    Skills       = request.Skills ?? string.Empty,
                    Title        = request.Title ?? string.Empty,
                    UserType     = normalizedUserType,
                    CompanyName  = request.CompanyName,
                    Description  = request.Description,
                    Industry     = request.Industry,
                    CompanySize  = request.CompanySize,
                    Website      = request.Website,
                    OtpCode      = otp,
                    ExpiresAt    = PendingRegistrationStore.NewExpiry()
                };

                _pendingStore.Create(pending);

                // Send OTP — non-blocking so slow email doesn't hang the response
                _ = _emailService.SendOtpAsync(normalizedEmail, request.FirstName, otp);

                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogInformation("OTP initiated for {Email}", normalizedEmail);

                return Ok(new { message = "Verification code sent. Please check your email.", email = normalizedEmail });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "RegisterInitiate failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── Step 2: Verify OTP → save user to DB, return JWT ────────────────
        [HttpPost("register/verify")]
        public async Task<IActionResult> RegisterVerify([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();
                var (pending, error) = _pendingStore.Verify(normalizedEmail, request.Otp);

                if (error != null)
                    return BadRequest(new { error });

                // Final duplicate check (tiny race-condition guard)
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
                if (existingUser != null)
                    return Conflict("An account with this email already exists.");

                Company? company = null;

                if (pending!.UserType == "Employer")
                {
                    company = new Company
                    {
                        Name        = pending.CompanyName!,
                        Description = pending.Description ?? string.Empty,
                        Location    = pending.Location,
                        Industry    = pending.Industry ?? string.Empty,
                        Size        = pending.CompanySize ?? string.Empty,
                        Website     = pending.Website ?? string.Empty,
                        Email       = pending.Email
                    };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }

                var user = new User
                {
                    FirstName    = pending.FirstName,
                    LastName     = pending.LastName,
                    Email        = pending.Email,
                    Phone        = pending.Phone,
                    Location     = pending.Location,
                    Bio          = pending.Bio,
                    Skills       = pending.Skills,
                    Title        = pending.Title,
                    UserType     = pending.UserType,
                    CompanyId    = company?.Id,
                    PasswordHash = pending.HashedPassword
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var response = new UserResponse
                {
                    Id          = user.Id,
                    FirstName   = user.FirstName,
                    LastName    = user.LastName,
                    Email       = user.Email,
                    Phone       = user.Phone,
                    Location    = user.Location,
                    Bio         = user.Bio,
                    Skills      = user.Skills,
                    Title       = user.Title,
                    UserType    = user.UserType,
                    Initials    = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive    = user.IsActive,
                    IsAdmin     = user.IsAdmin
                };

                _ = _emailService.SendWelcomeAsync(user.Email, user.FirstName, user.UserType);

                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogInformation("User {Email} verified and created (Id={UserId})", user.Email, user.Id);

                var token = _jwtService.GenerateToken(user);
                return StatusCode(201, new { token, user = response });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "RegisterVerify failed");
                return StatusCode(500, "An error occurred during registration. Please try again.");
            }
        }

        // ─── Resend OTP (refreshes code, keeps existing pending data) ─────────
        [HttpPost("register/resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            try
            {
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();
                var refreshed = _pendingStore.Refresh(normalizedEmail);

                if (refreshed == null)
                    return BadRequest(new { error = "No pending registration found. Please fill in the registration form again." });

                _ = _emailService.SendOtpAsync(normalizedEmail, refreshed.FirstName, refreshed.OtpCode);

                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogInformation("OTP resent for {Email}", normalizedEmail);

                return Ok(new { message = "A new verification code has been sent to your email." });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "ResendOtp failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail([FromQuery] string to = "grant88271@gmail.com")
        {
            try
            {
                var result = await _emailService.SendTestAsync(to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "TestEmail failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Normalize email (trim and lowercase)
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.IsActive);
                
                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid email or password");
                }
                
                // Load linked preferences and resume via raw SQL
                var prefsL = await ReadJobPreferencesRawAsync(user.Id);
                var resumeUrlL = await ReadResumeUrlRawAsync(user.Id);

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Location = user.Location,
                    Bio = user.Bio,
                    Skills = user.Skills,
                    Title = user.Title,
                    UserType = user.UserType,
                    Initials = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive = user.IsActive,
                    IsAdmin = user.IsAdmin,
                    ResumeUrl = resumeUrlL ?? user.ResumeUrl,
                    DesiredJobTitle = prefsL?.DesiredJobTitle ?? user.DesiredJobTitle,
                    DesiredLocation = prefsL?.DesiredLocation ?? user.DesiredLocation,
                    DesiredSalaryRange = prefsL?.DesiredSalaryRange ?? user.DesiredSalaryRange,
                    DesiredJobType = prefsL?.DesiredJobType ?? user.DesiredJobType,
                    DesiredExperienceLevel = prefsL?.DesiredExperienceLevel ?? user.DesiredExperienceLevel,
                    OpenToRemote = prefsL.HasValue ? prefsL.Value.OpenToRemote : user.OpenToRemote,
                    PreferredIndustries = prefsL?.PreferredIndustries ?? user.PreferredIndustries,
                    EducationLevel = prefsL?.EducationLevel ?? user.EducationLevel
                };

                // Generate a signed JWT
                var token = _jwtService.GenerateToken(user);

                var response = new
                {
                    token = token,
                    user = userResponse
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "Login failed");
                return StatusCode(500, "An error occurred during login. Please try again.");
            }
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
        {
            try
            {
                if (!IsAuthorizedForUser(id))
                    return Forbid();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
                
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var prefsG = await ReadJobPreferencesRawAsync(user.Id);
                var resumeUrlG = await ReadResumeUrlRawAsync(user.Id);

                var response = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Location = user.Location,
                    Bio = user.Bio,
                    Skills = user.Skills,
                    Title = user.Title,
                    UserType = user.UserType,
                    Initials = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive = user.IsActive,
                    ResumeUrl = resumeUrlG ?? user.ResumeUrl,
                    DesiredJobTitle = prefsG?.DesiredJobTitle ?? user.DesiredJobTitle,
                    DesiredLocation = prefsG?.DesiredLocation ?? user.DesiredLocation,
                    DesiredSalaryRange = prefsG?.DesiredSalaryRange ?? user.DesiredSalaryRange,
                    DesiredJobType = prefsG?.DesiredJobType ?? user.DesiredJobType,
                    DesiredExperienceLevel = prefsG?.DesiredExperienceLevel ?? user.DesiredExperienceLevel,
                    OpenToRemote = prefsG.HasValue ? prefsG.Value.OpenToRemote : user.OpenToRemote,
                    PreferredIndustries = prefsG?.PreferredIndustries ?? user.PreferredIndustries,
                    EducationLevel = prefsG?.EducationLevel ?? user.EducationLevel
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "GetUser failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpPut("profile/{id}")]
        public async Task<ActionResult<UserResponse>> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                if (!IsAuthorizedForUser(id))
                    return Forbid();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
                
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.FirstName))
                    user.FirstName = request.FirstName;
                
                if (!string.IsNullOrEmpty(request.LastName))
                    user.LastName = request.LastName;
                
                if (request.Phone != null)
                    user.Phone = request.Phone;
                
                if (request.Location != null)
                    user.Location = request.Location;
                
                if (request.Bio != null)
                    user.Bio = request.Bio;
                
                if (request.Skills != null)
                    user.Skills = request.Skills;
                
                if (request.Title != null)
                    user.Title = request.Title;
                
                // Update job preferences
                if (request.DesiredJobTitle != null)
                    user.DesiredJobTitle = request.DesiredJobTitle;
                
                if (request.DesiredLocation != null)
                    user.DesiredLocation = request.DesiredLocation;
                
                if (request.DesiredSalaryRange != null)
                    user.DesiredSalaryRange = request.DesiredSalaryRange;
                
                if (request.DesiredJobType != null)
                    user.DesiredJobType = request.DesiredJobType;
                
                if (request.DesiredExperienceLevel != null)
                    user.DesiredExperienceLevel = request.DesiredExperienceLevel;
                
                if (request.EducationLevel != null)
                    user.EducationLevel = request.EducationLevel;
                
                if (request.OpenToRemote.HasValue)
                    user.OpenToRemote = request.OpenToRemote.Value;
                
                if (request.PreferredIndustries != null)
                    user.PreferredIndustries = request.PreferredIndustries;

                user.UpdatedDate = DateTime.UtcNow;

                // Upsert Job_Preferences via raw SQL
                await UpsertJobPreferencesRawAsync(id, request);

                await _context.SaveChangesAsync();

                // Re-read from tables for the response
                var prefsU = await ReadJobPreferencesRawAsync(id);
                var resumeUrlU = await ReadResumeUrlRawAsync(id);

                var response = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Location = user.Location,
                    Bio = user.Bio,
                    Skills = user.Skills,
                    Title = user.Title,
                    UserType = user.UserType,
                    Initials = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive = user.IsActive,
                    ResumeUrl = resumeUrlU ?? user.ResumeUrl,
                    DesiredJobTitle = prefsU?.DesiredJobTitle,
                    DesiredLocation = prefsU?.DesiredLocation,
                    DesiredSalaryRange = prefsU?.DesiredSalaryRange,
                    DesiredJobType = prefsU?.DesiredJobType,
                    DesiredExperienceLevel = prefsU?.DesiredExperienceLevel,
                    OpenToRemote = prefsU.HasValue ? prefsU.Value.OpenToRemote : user.OpenToRemote,
                    PreferredIndustries = prefsU?.PreferredIndustries,
                    EducationLevel = prefsU?.EducationLevel
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "UpdateProfile failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpPost("profile/{id}/resume")]
        public async Task<ActionResult<UserResponse>> UploadResume(string id, [FromBody] ResumeUploadRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
                
                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.ResumeUrl = request.ResumeUrl;
                user.UpdatedDate = DateTime.UtcNow;

                // Also update all existing applications for this worker so employers see the latest resume
                var workerApplications = await _context.Applications
                    .Where(a => a.WorkerId == id)
                    .ToListAsync();
                foreach (var application in workerApplications)
                {
                    application.ResumeUrl = request.ResumeUrl;
                }

                await _context.SaveChangesAsync();

                var response = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Location = user.Location,
                    Bio = user.Bio,
                    Skills = user.Skills,
                    Title = user.Title,
                    UserType = user.UserType,
                    Initials = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive = user.IsActive,
                    ResumeUrl = user.ResumeUrl,
                    DesiredJobTitle = user.DesiredJobTitle,
                    DesiredLocation = user.DesiredLocation,
                    DesiredSalaryRange = user.DesiredSalaryRange,
                    DesiredJobType = user.DesiredJobType,
                    DesiredExperienceLevel = user.DesiredExperienceLevel,
                    OpenToRemote = user.OpenToRemote,
                    PreferredIndustries = user.PreferredIndustries
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "UploadResume failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpPost("profile/{id}/resume/upload")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<ActionResult<UserResponse>> UploadResumeFile(string id, [FromForm] IFormFile file)
        {
            try
            {
                if (!IsAuthorizedForUser(id))
                    return Forbid();

                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Only PDF, DOC, and DOCX files are allowed");

                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size must be under 5MB");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
                if (user == null)
                    return NotFound("User not found");

                // Save file to uploads/resumes/{userId}/
                var uploadsRoot = Environment.GetEnvironmentVariable("UPLOADS_PATH")
                    ?? Path.Combine(_environment.ContentRootPath, "uploads");
                var resumesPath = Path.Combine(uploadsRoot, "resumes", id);
                Directory.CreateDirectory(resumesPath);

                // Delete any old resume files for this user
                foreach (var oldFile in Directory.GetFiles(resumesPath))
                    System.IO.File.Delete(oldFile);

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(resumesPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var resumeUrl = $"/uploads/resumes/{id}/{fileName}";
                user.ResumeUrl = resumeUrl;
                user.UpdatedDate = DateTime.UtcNow;

                // Upsert into Resume table via raw SQL
                await UpsertResumeRawAsync(id, resumeUrl, file.FileName);

                // Update all existing applications so employers immediately see the new resume
                var workerApplications = await _context.Applications
                    .Where(a => a.WorkerId == id)
                    .ToListAsync();
                foreach (var application in workerApplications)
                {
                    application.ResumeUrl = resumeUrl;
                }

                await _context.SaveChangesAsync();

                var prefsForResume = await ReadJobPreferencesRawAsync(id);

                var response = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Location = user.Location,
                    Bio = user.Bio,
                    Skills = user.Skills,
                    Title = user.Title,
                    UserType = user.UserType,
                    Initials = user.Initials,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    IsActive = user.IsActive,
                    ResumeUrl = resumeUrl,
                    DesiredJobTitle = prefsForResume?.DesiredJobTitle ?? user.DesiredJobTitle,
                    DesiredLocation = prefsForResume?.DesiredLocation ?? user.DesiredLocation,
                    DesiredSalaryRange = prefsForResume?.DesiredSalaryRange ?? user.DesiredSalaryRange,
                    DesiredJobType = prefsForResume?.DesiredJobType ?? user.DesiredJobType,
                    DesiredExperienceLevel = prefsForResume?.DesiredExperienceLevel ?? user.DesiredExperienceLevel,
                    OpenToRemote = prefsForResume.HasValue ? prefsForResume.Value.OpenToRemote : user.OpenToRemote,
                    PreferredIndustries = prefsForResume?.PreferredIndustries ?? user.PreferredIndustries
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "UploadResumeFile failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── Forgot Password ─────────────────────────────────────────────────

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var email = request.Email.Trim().ToLowerInvariant();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

                // Always return 200 so we don't leak whether the email exists
                if (user == null)
                    return Ok(new { message = "If an account with that email exists, a reset link has been sent." });

                // Generate a secure random token
                var tokenBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
                var token = Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-").Replace("/", "_").Replace("=", ""); // URL-safe

                user.PasswordResetToken = token;
                user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                var resetLink = $"{_frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
                _ = _emailService.SendPasswordResetAsync(user.Email, user.FirstName, resetLink);

                return Ok(new { message = "If an account with that email exists, a reset link has been sent." });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "ForgotPassword failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── Reset Password ──────────────────────────────────────────────────

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == request.Token &&
                    u.PasswordResetExpiry != null &&
                    u.PasswordResetExpiry > DateTime.UtcNow &&
                    u.IsActive);

                if (user == null)
                    return BadRequest(new { message = "This reset link is invalid or has expired." });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetExpiry = null;
                user.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Your password has been reset. You can now log in." });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "ResetPassword failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── GET /api/auth/company/{userId} ───────────────────────────────────
        [Authorize]
        [HttpGet("company/{userId}")]
        public async Task<IActionResult> GetCompanyProfile(string userId)
        {
            if (!IsAuthorizedForUser(userId))
                return Forbid();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (user.Company == null)
                    return NotFound(new { message = "No company linked to this account." });

                var c = user.Company;
                return Ok(new
                {
                    companyName  = c.Name,
                    industry     = c.Industry,
                    companySize  = c.Size,
                    website      = c.Website,
                    location     = c.Location,
                    description  = c.Description,
                    contactEmail = c.Email
                });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "GetCompanyProfile failed for userId={UserId}", userId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── PUT /api/auth/company/{userId} ───────────────────────────────────
        [Authorize]
        [HttpPut("company/{userId}")]
        public async Task<IActionResult> UpdateCompanyProfile(string userId, [FromBody] UpdateCompanyRequest request)
        {
            if (!IsAuthorizedForUser(userId))
                return Forbid();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (user.Company == null)
                    return NotFound(new { message = "No company linked to this account." });

                var c = user.Company;
                c.Name        = request.CompanyName?.Trim() ?? c.Name;
                c.Industry    = request.Industry?.Trim() ?? c.Industry;
                c.Size        = request.CompanySize?.Trim() ?? c.Size;
                c.Website     = request.Website?.Trim() ?? c.Website;
                c.Location    = request.Location?.Trim() ?? c.Location;
                c.Description = request.Description?.Trim() ?? c.Description;
                c.Email       = request.ContactEmail?.Trim() ?? c.Email;
                c.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    companyName  = c.Name,
                    industry     = c.Industry,
                    companySize  = c.Size,
                    website      = c.Website,
                    location     = c.Location,
                    description  = c.Description,
                    contactEmail = c.Email
                });
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
                logger.LogError(ex, "UpdateCompanyProfile failed for userId={UserId}", userId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if the current JWT belongs to the given userId, or if the caller is an admin.
        /// </summary>
        private bool IsAuthorizedForUser(string userId)
        {
            var tokenUserId = User.GetUserId();
            var isAdmin = User.GetIsAdmin();
            return isAdmin || tokenUserId == userId;
        }

        // ── Raw SQL helpers for Job_Preferences and Resume tables ─────────────
        // These bypass EF Core model tracking so they work even without a formal
        // EF Core migration — the tables are created in Program.cs startup SQL.

        private async Task<(string? DesiredJobTitle, string? DesiredLocation, string? DesiredSalaryRange,
            string? DesiredJobType, string? DesiredExperienceLevel, bool OpenToRemote,
            string? PreferredIndustries, string? EducationLevel)?> ReadJobPreferencesRawAsync(string userId)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await ((System.Data.Common.DbConnection)conn).OpenAsync();
                using var cmd = (System.Data.Common.DbCommand)conn.CreateCommand();
                cmd.CommandText = @"SELECT ""DesiredJobTitle"",""DesiredLocation"",""DesiredSalaryRange"",
                    ""DesiredJobType"",""DesiredExperienceLevel"",""OpenToRemote"",
                    ""PreferredIndustries"",""EducationLevel""
                    FROM ""Job_Preferences"" WHERE ""UserId""=@uid LIMIT 1";
                var p = cmd.CreateParameter(); p.ParameterName = "@uid"; p.Value = userId; cmd.Parameters.Add(p);
                using var r = await cmd.ExecuteReaderAsync();
                if (await r.ReadAsync())
                    return (
                        r.IsDBNull(0) ? null : r.GetString(0),
                        r.IsDBNull(1) ? null : r.GetString(1),
                        r.IsDBNull(2) ? null : r.GetString(2),
                        r.IsDBNull(3) ? null : r.GetString(3),
                        r.IsDBNull(4) ? null : r.GetString(4),
                        !r.IsDBNull(5) && r.GetBoolean(5),
                        r.IsDBNull(6) ? null : r.GetString(6),
                        r.IsDBNull(7) ? null : r.GetString(7)
                    );
                return null;
            }
            catch { return null; }
        }

        private async Task<string?> ReadResumeUrlRawAsync(string userId)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await ((System.Data.Common.DbConnection)conn).OpenAsync();
                using var cmd = (System.Data.Common.DbCommand)conn.CreateCommand();
                cmd.CommandText = "SELECT \"FileUrl\" FROM \"Resume\" WHERE \"UserId\"=@uid LIMIT 1";
                var p = cmd.CreateParameter(); p.ParameterName = "@uid"; p.Value = userId; cmd.Parameters.Add(p);
                var result = await cmd.ExecuteScalarAsync();
                return result is DBNull || result == null ? null : (string)result;
            }
            catch { return null; }
        }

        private static void AddParam(System.Data.Common.DbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? (object)DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private async Task UpsertJobPreferencesRawAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await ((System.Data.Common.DbConnection)conn).OpenAsync();
                using var cmd = (System.Data.Common.DbCommand)conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO ""Job_Preferences"" (""Id"",""UserId"",""DesiredJobTitle"",""DesiredLocation"",
                        ""DesiredSalaryRange"",""DesiredJobType"",""DesiredExperienceLevel"",
                        ""OpenToRemote"",""PreferredIndustries"",""EducationLevel"",""UpdatedDate"")
                    VALUES (@id,@uid,@jt,@dl,@sr,@jtype,@el,@otr,@pi,@edu,@upd)
                    ON CONFLICT (""UserId"") DO UPDATE SET
                        ""DesiredJobTitle"" = COALESCE(@jt, ""Job_Preferences"".""DesiredJobTitle""),
                        ""DesiredLocation"" = COALESCE(@dl, ""Job_Preferences"".""DesiredLocation""),
                        ""DesiredSalaryRange"" = COALESCE(@sr, ""Job_Preferences"".""DesiredSalaryRange""),
                        ""DesiredJobType"" = COALESCE(@jtype, ""Job_Preferences"".""DesiredJobType""),
                        ""DesiredExperienceLevel"" = COALESCE(@el, ""Job_Preferences"".""DesiredExperienceLevel""),
                        ""OpenToRemote"" = CASE WHEN @otrprovided THEN @otr ELSE ""Job_Preferences"".""OpenToRemote"" END,
                        ""PreferredIndustries"" = COALESCE(@pi, ""Job_Preferences"".""PreferredIndustries""),
                        ""EducationLevel"" = COALESCE(@edu, ""Job_Preferences"".""EducationLevel""),
                        ""UpdatedDate"" = @upd";
                AddParam(cmd, "@id", Guid.NewGuid().ToString());
                AddParam(cmd, "@uid", userId);
                AddParam(cmd, "@jt", request.DesiredJobTitle);
                AddParam(cmd, "@dl", request.DesiredLocation);
                AddParam(cmd, "@sr", request.DesiredSalaryRange);
                AddParam(cmd, "@jtype", request.DesiredJobType);
                AddParam(cmd, "@el", request.DesiredExperienceLevel);
                AddParam(cmd, "@otr", request.OpenToRemote.HasValue ? (object)request.OpenToRemote.Value : DBNull.Value);
                AddParam(cmd, "@otrprovided", request.OpenToRemote.HasValue);
                AddParam(cmd, "@pi", request.PreferredIndustries);
                AddParam(cmd, "@edu", request.EducationLevel);
                AddParam(cmd, "@upd", DateTime.UtcNow);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { Console.WriteLine($"Warning: UpsertJobPreferences failed: {ex.Message}"); }
        }

        private async Task UpsertResumeRawAsync(string userId, string fileUrl, string fileName)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await ((System.Data.Common.DbConnection)conn).OpenAsync();
                using var cmd = (System.Data.Common.DbCommand)conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO ""Resume"" (""Id"",""UserId"",""FileUrl"",""FileName"",""UploadedDate"")
                    VALUES (@id,@uid,@url,@name,@upd)
                    ON CONFLICT (""UserId"") DO UPDATE SET
                        ""FileUrl"" = @url,
                        ""FileName"" = @name,
                        ""UploadedDate"" = @upd";
                AddParam(cmd, "@id", Guid.NewGuid().ToString());
                AddParam(cmd, "@uid", userId);
                AddParam(cmd, "@url", fileUrl);
                AddParam(cmd, "@name", fileName);
                AddParam(cmd, "@upd", DateTime.UtcNow);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { Console.WriteLine($"Warning: UpsertResume failed: {ex.Message}"); }
        }

        private static string NormalizeUserType(string? userType)
        {
            if (string.IsNullOrWhiteSpace(userType))
            {
                return "Worker";
            }

            var normalized = userType.Trim();
            if (normalized.Equals("employer", StringComparison.OrdinalIgnoreCase))
            {
                return "Employer";
            }

            if (normalized.Equals("worker", StringComparison.OrdinalIgnoreCase))
            {
                return "Worker";
            }

            return normalized;
        }

        private static string NormalizeFrontendUrl(string? configuredFrontendUrl)
        {
            const string defaultFrontendUrl = "https://www.rightfitgigs.com";

            if (string.IsNullOrWhiteSpace(configuredFrontendUrl))
            {
                return defaultFrontendUrl;
            }

            var trimmedUrl = configuredFrontendUrl.Trim().TrimEnd('/');

            if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri))
            {
                return defaultFrontendUrl;
            }

            if (uri.Host.Equals("rightfitgigs.com", StringComparison.OrdinalIgnoreCase))
            {
                return $"{uri.Scheme}://www.rightfitgigs.com";
            }

            return trimmedUrl;
        }

        // ─── Delete Account ───────────────────────────────────────────────────
        [Authorize]
        [HttpDelete("account/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

            try
            {
                var requestingUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                    ?? User.FindFirst("sub")?.Value;

                if (requestingUserId != id)
                    return Forbid();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                    return NotFound("User not found");

                // Use raw SQL with an explicit transaction to guarantee correct delete order
                // and avoid EF Core change-tracker cascade ordering surprises.
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. If employer: delete applications on their jobs first, then the jobs
                    if (user.UserType == "Employer")
                    {
                        await _context.Database.ExecuteSqlRawAsync(
                            @"DELETE FROM ""Applications"" WHERE ""JobId"" IN (SELECT ""Id"" FROM ""Jobs"" WHERE ""EmployerId"" = {0})",
                            id);
                        await _context.Database.ExecuteSqlRawAsync(
                            @"DELETE FROM ""Jobs"" WHERE ""EmployerId"" = {0}",
                            id);
                    }

                    // 2. Delete applications where this user is the worker
                    await _context.Database.ExecuteSqlRawAsync(
                        @"DELETE FROM ""Applications"" WHERE ""WorkerId"" = {0}",
                        id);

                    // 3. Delete messages (no FK constraint, plain string columns)
                    await _context.Database.ExecuteSqlRawAsync(
                        @"DELETE FROM ""Messages"" WHERE ""SenderId"" = {0} OR ""ReceiverId"" = {0}",
                        id);

                    // 4. Delete notifications (no FK constraint, plain string column)
                    await _context.Database.ExecuteSqlRawAsync(
                        @"DELETE FROM ""Notifications"" WHERE ""UserId"" = {0}",
                        id);

                    // 5. Delete the user row — PostgreSQL ON DELETE CASCADE handles Job_Preferences and Resume
                    await _context.Database.ExecuteSqlRawAsync(
                        @"DELETE FROM ""Users"" WHERE ""Id"" = {0}",
                        id);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return Ok(new { message = "Account deleted successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteAccount failed for id {Id}", id);
                // Include error details to help diagnose issues
                return StatusCode(500, new { message = "An error occurred while deleting the account.", detail = ex.Message });
            }
        }
    }

    public class ResumeUploadRequest
    {
        [Required]
        [StringLength(500)]
        public string ResumeUrl { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Location { get; set; }
        
        [StringLength(1000)]
        public string? Bio { get; set; }
        
        [StringLength(500)]
        public string? Skills { get; set; }
        
        [StringLength(100)]
        public string? Title { get; set; }
        
        [Required]
        [StringLength(10)]
        public string UserType { get; set; } = "Worker";
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        // Company fields (for Employer registration)
        [StringLength(100)]
        public string? CompanyName { get; set; }
        
        [StringLength(20)]
        public string? CompanySize { get; set; }
        
        [StringLength(100)]
        public string? Industry { get; set; }
        
        [StringLength(200)]
        public string? Website { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; } = string.Empty;
    }

    public class ResendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateCompanyRequest
    {
        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(20)]
        public string? CompanySize { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? ContactEmail { get; set; }
    }
}