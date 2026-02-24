using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;
using System.ComponentModel.DataAnnotations;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Normalize email (trim and lowercase)
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();
                var normalizedUserType = NormalizeUserType(request.UserType);

                if (normalizedUserType != "Worker" && normalizedUserType != "Employer")
                {
                    return BadRequest("User type must be Worker or Employer");
                }

                // Check if user with email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
                if (existingUser != null)
                {
                    return Conflict($"User with email {request.Email} already exists");
                }

                Company? company = null;
                
                // If registering as Employer, create company first
                if (normalizedUserType == "Employer")
                {
                    if (string.IsNullOrWhiteSpace(request.CompanyName))
                    {
                        return BadRequest("Company name is required for employer registration");
                    }
                    
                    company = new Company
                    {
                        Name = request.CompanyName,
                        Description = request.Description ?? string.Empty,
                        Location = request.Location ?? string.Empty,
                        Industry = request.Industry ?? string.Empty,
                        Size = request.CompanySize ?? string.Empty,
                        Website = request.Website ?? string.Empty,
                        Email = request.Email
                    };
                    
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = normalizedEmail,
                    Phone = request.Phone ?? string.Empty,
                    Location = request.Location ?? string.Empty,
                    Bio = request.Bio ?? string.Empty,
                    Skills = request.Skills ?? string.Empty,
                    Title = request.Title ?? string.Empty,
                    UserType = normalizedUserType,
                    CompanyId = company?.Id,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.Users.Add(user);
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
                    IsAdmin = user.IsAdmin
                };

                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                    IsAdmin = user.IsAdmin
                };

                // Generate a simple token (in production, use JWT)
                var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.Id}:{DateTime.UtcNow.Ticks}"));

                var response = new
                {
                    token = token,
                    user = userResponse
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
                
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

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
                    IsActive = user.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("profile/{id}")]
        public async Task<ActionResult<UserResponse>> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
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
                
                if (request.OpenToRemote.HasValue)
                    user.OpenToRemote = request.OpenToRemote.Value;
                
                if (request.PreferredIndustries != null)
                    user.PreferredIndustries = request.PreferredIndustries;

                user.UpdatedDate = DateTime.UtcNow;
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
}