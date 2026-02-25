using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationResponse>> SubmitApplication([FromBody] ApplicationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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
                    ResumeUrl = worker.ResumeUrl,
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetAllApplications()
        {
            try
            {
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                    return NotFound($"Application with ID {id} not found");
                }

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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("worker/{workerId}")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetWorkerApplications(string workerId)
        {
            try
            {
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetJobApplications(string jobId)
        {
            try
            {
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                    return NotFound($"Application with ID {id} not found");
                }

                application.Status = request.Status;
                application.UpdatedDate = DateTime.UtcNow;

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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
