using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;
using RightFitGigs.Services;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ApplicationsController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApplicationResponse>> SubmitApplication([FromBody] ApplicationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate the submitting worker matches the token
                var tokenUserId = User.GetUserId();
                if (tokenUserId != request.WorkerId && !User.GetIsAdmin())
                    return Forbid();

                // Check if user already applied for this job
                var existingApplication = await _context.Applications
                    .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.WorkerId == request.WorkerId);
                
                if (existingApplication != null)
                {
                    return Conflict("You have already applied for this job");
                }

                // Get worker details
                var worker = await _context.Users.FindAsync(request.WorkerId);
                if (worker == null)
                {
                    return NotFound("Worker not found");
                }

                // Get job details
                var job = await _context.Jobs.FindAsync(request.JobId);
                if (job == null)
                {
                    return NotFound("Job not found");
                }

                // Prefer the dedicated Resume table; fall back to the Users column
                var resumeUrl = worker.ResumeUrl;
                if (string.IsNullOrEmpty(resumeUrl))
                {
                    try
                    {
                        var conn = _context.Database.GetDbConnection();
                        if (conn.State != System.Data.ConnectionState.Open)
                            await ((System.Data.Common.DbConnection)conn).OpenAsync();
                        using var cmd = (System.Data.Common.DbCommand)conn.CreateCommand();
                        cmd.CommandText = @"SELECT ""FileUrl"" FROM ""Resume"" WHERE ""UserId""=@uid LIMIT 1";
                        var p = cmd.CreateParameter(); p.ParameterName = "@uid"; p.Value = worker.Id; cmd.Parameters.Add(p);
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                            resumeUrl = result.ToString();
                    }
                    catch { /* non-critical — proceed without resume URL */ }
                }

                var application = new Application
                {
                    JobId = request.JobId,
                    WorkerId = request.WorkerId,
                    WorkerName = $"{worker.FirstName} {worker.LastName}",
                    WorkerEmail = worker.Email,
                    WorkerPhone = worker.Phone,
                    WorkerSkills = worker.Skills,
                    WorkerTitle = worker.Title,
                    WorkerLocation = worker.Location,
                    ResumeUrl = resumeUrl,
                    CoverLetter = request.CoverLetter ?? string.Empty,
                    Status = "Pending"
                };

                _context.Applications.Add(application);
                
                // Create notification for the employer(s) of the company
                if (!string.IsNullOrEmpty(job.CompanyId))
                {
                    var employers = await _context.Users
                        .Where(u => u.CompanyId == job.CompanyId && u.UserType == "Employer")
                        .ToListAsync();

                    foreach (var employer in employers)
                    {
                        var notification = new Notification
                        {
                            UserId = employer.Id,
                            Type = "NewApplication",
                            Title = "Potential candidate applied for a position",
                            Message = $"{application.WorkerName} applied for {job.Title}",
                            RelatedId = application.Id,
                            JobId = job.Id,
                            JobTitle = job.Title
                        };
                        _context.Notifications.Add(notification);
                    }
                }
                
                await _context.SaveChangesAsync();

                var response = new ApplicationResponse
                {
                    Id = application.Id,
                    JobId = application.JobId,
                    JobTitle = job.Title,
                    Company = job.Company,
                    WorkerId = application.WorkerId,
                    WorkerName = application.WorkerName,
                    WorkerEmail = application.WorkerEmail,
                    WorkerPhone = application.WorkerPhone,
                    WorkerSkills = application.WorkerSkills,
                    WorkerTitle = application.WorkerTitle,
                    WorkerLocation = application.WorkerLocation,
                    ResumeUrl = application.ResumeUrl,
                    CoverLetter = application.CoverLetter,
                    Status = application.Status,
                    AppliedDate = application.AppliedDate,
                    UpdatedDate = application.UpdatedDate
                };

                return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "SubmitApplication failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetAllApplications()
        {
            try
            {
                // Only admins may view all applications
                if (!User.GetIsAdmin())
                    return Forbid();
                var applications = await _context.Applications
                    .Include(a => a.Job)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

                var response = applications.Select(a => new ApplicationResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job?.Title ?? string.Empty,
                    Company = a.Job?.Company ?? string.Empty,
                    WorkerId = a.WorkerId,
                    WorkerName = a.WorkerName,
                    WorkerEmail = a.WorkerEmail,
                    WorkerPhone = a.WorkerPhone,
                    WorkerSkills = a.WorkerSkills,
                    WorkerTitle = a.WorkerTitle,
                    WorkerLocation = a.WorkerLocation,
                    ResumeUrl = a.ResumeUrl,
                    CoverLetter = a.CoverLetter,
                    Status = a.Status,
                    AppliedDate = a.AppliedDate,
                    UpdatedDate = a.UpdatedDate
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "GetAllApplications failed");
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationResponse>> GetApplication(string id)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (application == null)
                {
                    return NotFound("Application not found");
                }

                // Only the worker who applied, the employer who owns the job, or admin may view
                var tokenUserId = User.GetUserId();
                var isAdmin = User.GetIsAdmin();
                var ownsJob = application.Job != null && (
                    application.Job.EmployerId == tokenUserId ||
                    (!string.IsNullOrEmpty(application.Job.CompanyId) && await _context.Users
                        .AnyAsync(u => u.Id == tokenUserId && u.CompanyId == application.Job.CompanyId))
                );

                if (!isAdmin && tokenUserId != application.WorkerId && !ownsJob)
                    return Forbid();

                var response = new ApplicationResponse
                {
                    Id = application.Id,
                    JobId = application.JobId,
                    JobTitle = application.Job?.Title ?? string.Empty,
                    Company = application.Job?.Company ?? string.Empty,
                    WorkerId = application.WorkerId,
                    WorkerName = application.WorkerName,
                    WorkerEmail = application.WorkerEmail,
                    WorkerPhone = application.WorkerPhone,
                    WorkerSkills = application.WorkerSkills,
                    WorkerTitle = application.WorkerTitle,
                    WorkerLocation = application.WorkerLocation,
                    ResumeUrl = application.ResumeUrl,
                    CoverLetter = application.CoverLetter,
                    Status = application.Status,
                    AppliedDate = application.AppliedDate,
                    UpdatedDate = application.UpdatedDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "GetApplication failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpGet("worker/{workerId}")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetWorkerApplications(string workerId)
        {
            try
            {
                // Workers may only view their own applications; admins may view any
                var tokenUserId = User.GetUserId();
                if (tokenUserId != workerId && !User.GetIsAdmin())
                    return Forbid();
                var applications = await _context.Applications
                    .Include(a => a.Job)
                    .Where(a => a.WorkerId == workerId)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

                var response = applications.Select(a => new ApplicationResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job?.Title ?? string.Empty,
                    Company = a.Job?.Company ?? string.Empty,
                    WorkerId = a.WorkerId,
                    WorkerName = a.WorkerName,
                    WorkerEmail = a.WorkerEmail,
                    WorkerPhone = a.WorkerPhone,
                    WorkerSkills = a.WorkerSkills,
                    WorkerTitle = a.WorkerTitle,
                    WorkerLocation = a.WorkerLocation,
                    ResumeUrl = a.ResumeUrl,
                    CoverLetter = a.CoverLetter,
                    Status = a.Status,
                    AppliedDate = a.AppliedDate,
                    UpdatedDate = a.UpdatedDate
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "GetWorkerApplications failed for workerId {WorkerId}", workerId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetJobApplications(string jobId)
        {
            try
            {
                // Only the employer who owns the job, or admin, may view applicants
                var tokenUserId = User.GetUserId();
                var isAdmin = User.GetIsAdmin();
                var job = await _context.Jobs.FindAsync(jobId);
                if (job == null) return NotFound("Job not found");

                var ownsJob = job.EmployerId == tokenUserId ||
                    (!string.IsNullOrEmpty(job.CompanyId) && await _context.Users
                        .AnyAsync(u => u.Id == tokenUserId && u.CompanyId == job.CompanyId));

                if (!isAdmin && !ownsJob)
                    return Forbid();
                var applications = await _context.Applications
                    .Include(a => a.Job)
                    .Where(a => a.JobId == jobId)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

                var response = applications.Select(a => new ApplicationResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job?.Title ?? string.Empty,
                    Company = a.Job?.Company ?? string.Empty,
                    WorkerId = a.WorkerId,
                    WorkerName = a.WorkerName,
                    WorkerEmail = a.WorkerEmail,
                    WorkerPhone = a.WorkerPhone,
                    WorkerSkills = a.WorkerSkills,
                    WorkerTitle = a.WorkerTitle,
                    WorkerLocation = a.WorkerLocation,
                    ResumeUrl = a.ResumeUrl,
                    CoverLetter = a.CoverLetter,
                    Status = a.Status,
                    AppliedDate = a.AppliedDate,
                    UpdatedDate = a.UpdatedDate
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "GetJobApplications failed for jobId {JobId}", jobId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        // Returns all applications across every job posted by the employer's company.
        // Accessible only by the employer themselves (or an admin).
        [Authorize]
        [HttpGet("employer/{employerId}")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetEmployerApplications(string employerId)
        {
            try
            {
                var tokenUserId = User.GetUserId();
                var isAdmin = User.GetIsAdmin();

                if (tokenUserId != employerId && !isAdmin)
                    return Forbid();

                // Resolve the employer's company
                var employer = await _context.Users.FindAsync(employerId);
                if (employer == null)
                    return NotFound("Employer not found");

                // Filter by EmployerId — CompanyId is not set on jobs, but EmployerId always is
                var applications = await _context.Applications
                    .Include(a => a.Job)
                    .Where(a => a.Job != null && a.Job.EmployerId == employerId)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

                var response = applications.Select(a => new ApplicationResponse
                {
                    Id            = a.Id,
                    JobId         = a.JobId,
                    JobTitle      = a.Job?.Title ?? string.Empty,
                    Company       = a.Job?.Company ?? string.Empty,
                    WorkerId      = a.WorkerId,
                    WorkerName    = a.WorkerName,
                    WorkerEmail   = a.WorkerEmail,
                    WorkerPhone   = a.WorkerPhone,
                    WorkerSkills  = a.WorkerSkills,
                    WorkerTitle   = a.WorkerTitle,
                    WorkerLocation = a.WorkerLocation,
                    ResumeUrl     = a.ResumeUrl,
                    CoverLetter   = a.CoverLetter,
                    Status        = a.Status,
                    AppliedDate   = a.AppliedDate,
                    UpdatedDate   = a.UpdatedDate
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "GetEmployerApplications failed for employerId {EmployerId}", employerId);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApplicationResponse>> UpdateApplicationStatus(
            string id, 
            [FromBody] UpdateApplicationStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var application = await _context.Applications
                    .Include(a => a.Job)
                    .FirstOrDefaultAsync(a => a.Id == id);
                
                if (application == null)
                {
                    return NotFound("Application not found");
                }

                // Only the employer who owns the job (by EmployerId or CompanyId), or admin, may change status
                var tokenUserId = User.GetUserId();
                var isAdmin = User.GetIsAdmin();
                var ownsJob = application.Job != null && (
                    application.Job.EmployerId == tokenUserId ||
                    (!string.IsNullOrEmpty(application.Job.CompanyId) && await _context.Users
                        .AnyAsync(u => u.Id == tokenUserId && u.CompanyId == application.Job.CompanyId))
                );

                if (!isAdmin && !ownsJob)
                    return Forbid();

                application.Status = request.Status;
                application.UpdatedDate = DateTime.UtcNow;

                // Fire status update email to worker (non-blocking)
                if (!string.IsNullOrEmpty(application.WorkerEmail))
                {
                    _ = _emailService.SendStatusUpdateAsync(
                        application.WorkerEmail,
                        application.WorkerName,
                        application.Job?.Title ?? "the position",
                        application.Job?.Company ?? "the company",
                        request.Status);
                }

                // Fire status update email to worker (non-blocking)
                if (!string.IsNullOrEmpty(application.WorkerEmail))
                {
                    _ = _emailService.SendStatusUpdateAsync(
                        application.WorkerEmail,
                        application.WorkerName,
                        application.Job?.Title ?? "the position",
                        application.Job?.Company ?? "the company",
                        request.Status);
                }

                // Create notification for the worker about status change
                var statusMessage = request.Status.ToLower() switch
                {
                    "reviewing" => $"Your application for {application.Job?.Title ?? "the position"} is being reviewed",
                    "accepted" => $"Congratulations! Your application for {application.Job?.Title ?? "the position"} has been accepted",
                    "rejected" => $"Your application for {application.Job?.Title ?? "the position"} was not selected at this time",
                    "interviewing" => $"You've been invited to interview for {application.Job?.Title ?? "the position"}",
                    "offer" => $"You've received an offer for {application.Job?.Title ?? "the position"}",
                    _ => $"Your application status has been updated to {request.Status}"
                };

                var notification = new Notification
                {
                    UserId = application.WorkerId,
                    Type = "ApplicationStatusUpdate",
                    Title = $"Application status: {request.Status}",
                    Message = statusMessage,
                    RelatedId = application.Id,
                    JobId = application.JobId,
                    JobTitle = application.Job?.Title
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var response = new ApplicationResponse
                {
                    Id = application.Id,
                    JobId = application.JobId,
                    JobTitle = application.Job?.Title ?? string.Empty,
                    Company = application.Job?.Company ?? string.Empty,
                    WorkerId = application.WorkerId,
                    WorkerName = application.WorkerName,
                    WorkerEmail = application.WorkerEmail,
                    WorkerPhone = application.WorkerPhone,
                    WorkerSkills = application.WorkerSkills,
                    WorkerTitle = application.WorkerTitle,
                    WorkerLocation = application.WorkerLocation,
                    ResumeUrl = application.ResumeUrl,
                    CoverLetter = application.CoverLetter,
                    Status = application.Status,
                    AppliedDate = application.AppliedDate,
                    UpdatedDate = application.UpdatedDate
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "UpdateApplicationStatus failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (application == null)
                {
                    return NotFound("Application not found");
                }

                var tokenUserId = User.GetUserId();
                var isAdmin = User.GetIsAdmin();
                var ownsApplication = application.WorkerId == tokenUserId;
                var ownsJob = application.Job != null && (
                    application.Job.EmployerId == tokenUserId ||
                    (!string.IsNullOrEmpty(application.Job.CompanyId) && await _context.Users
                        .AnyAsync(u => u.Id == tokenUserId && u.CompanyId == application.Job.CompanyId))
                );

                if (!isAdmin && !ownsApplication && !ownsJob)
                {
                    return Forbid();
                }

                var notifications = await _context.Notifications
                    .Where(n => n.RelatedId == application.Id)
                    .ToListAsync();

                if (notifications.Count > 0)
                {
                    _context.Notifications.RemoveRange(notifications);
                }

                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ApplicationsController>>();
                logger.LogError(ex, "DeleteApplication failed for id {Id}", id);
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }
    }
}
