using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobResponse>>> GetAllJobs([FromQuery] JobSearchRequest request)
        {
            try
            {
                // If an employerId is provided the employer is viewing their own jobs —
                // return all (active + suspended). For public views, only return active jobs.
                var query = _context.Jobs.AsQueryable();

                if (string.IsNullOrWhiteSpace(request.EmployerId))
                {
                    query = query.Where(j => j.IsActive);
                }

                // Apply search filters
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var searchTerm = request.Search.ToLower();
                    query = query.Where(j => 
                        j.Title.ToLower().Contains(searchTerm) ||
                        j.Company.ToLower().Contains(searchTerm) ||
                        j.Description.ToLower().Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(request.Location) && request.Location != "All Locations")
                {
                    query = query.Where(j => j.Location == request.Location);
                }

                if (!string.IsNullOrWhiteSpace(request.Type) && request.Type != "All Types")
                {
                    query = query.Where(j => j.Type == request.Type);
                }

                if (!string.IsNullOrWhiteSpace(request.Industry) && request.Industry != "All Industries")
                {
                    query = query.Where(j => j.Industry == request.Industry);
                }

                if (!string.IsNullOrWhiteSpace(request.ExperienceLevel) && request.ExperienceLevel != "All Levels")
                {
                    query = query.Where(j => j.ExperienceLevel == request.ExperienceLevel);
                }

                if (request.IsRemote.HasValue)
                {
                    query = query.Where(j => j.IsRemote == request.IsRemote.Value);
                }

                if (request.IsUrgentlyHiring.HasValue)
                {
                    query = query.Where(j => j.IsUrgentlyHiring == request.IsUrgentlyHiring.Value);
                }

                if (request.IsSeasonal.HasValue)
                {
                    query = query.Where(j => j.IsSeasonal == request.IsSeasonal.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.EmployerId))
                {
                    query = query.Where(j => j.EmployerId == request.EmployerId);
                }

                // Order by posted date (newest first)
                query = query.OrderByDescending(j => j.PostedDate);

                // Apply pagination
                var totalCount = await query.CountAsync();
                var jobs = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                var companyNamesByEmployerId = await GetCompanyNamesByEmployerIdAsync(jobs);

                var response = jobs.Select(j => new JobResponse
                {
                    Id = j.Id,
                    Title = j.Title,
                    Company = GetDisplayCompanyName(j, companyNamesByEmployerId),
                    Location = j.Location,
                    Description = j.Description,
                    Salary = j.Salary,
                    Type = j.Type,
                    Industry = j.Industry,
                    ExperienceLevel = j.ExperienceLevel,
                    EducationLevel = j.EducationLevel,
                    IsRemote = j.IsRemote,
                    IsUrgentlyHiring = j.IsUrgentlyHiring,
                    IsSeasonal = j.IsSeasonal,
                    PostedDate = j.PostedDate,
                    UpdatedDate = j.UpdatedDate,
                    IsActive = j.IsActive,
                    EmployerId = j.EmployerId
                }).ToList();

                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobResponse>> GetJob(string id)
        {
            try
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.IsActive);
                
                if (job == null)
                {
                    return NotFound($"Job with ID {id} not found");
                }
                var companyName = await GetCompanyNameForEmployerIdAsync(job.EmployerId);

                var response = new JobResponse
                {
                    Id = job.Id,
                    Title = job.Title,
                    Company = string.IsNullOrWhiteSpace(companyName) ? job.Company : companyName,
                    Location = job.Location,
                    Description = job.Description,
                    Salary = job.Salary,
                    Type = job.Type,
                    Industry = job.Industry,
                    ExperienceLevel = job.ExperienceLevel,
                    EducationLevel = job.EducationLevel,
                    IsRemote = job.IsRemote,
                    IsUrgentlyHiring = job.IsUrgentlyHiring,
                    IsSeasonal = job.IsSeasonal,
                    PostedDate = job.PostedDate,
                    UpdatedDate = job.UpdatedDate,
                    IsActive = job.IsActive,
                    EmployerId = job.EmployerId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<JobResponse>> CreateJob([FromBody] JobRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var companyName = await GetCompanyNameForEmployerIdAsync(request.EmployerId);
                if (!string.IsNullOrWhiteSpace(companyName))
                {
                    request.Company = companyName;
                }

                var job = new Job
                {
                    Title = request.Title,
                    Company = request.Company,
                    Location = request.Location,
                    Description = request.Description,
                    Salary = request.Salary,
                    Type = request.Type,
                    Industry = request.Industry,
                    ExperienceLevel = request.ExperienceLevel,
                    EducationLevel = request.EducationLevel,
                    IsRemote = request.IsRemote,
                    IsUrgentlyHiring = request.IsUrgentlyHiring,
                    IsSeasonal = request.IsSeasonal,
                    EmployerId = request.EmployerId
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                var response = new JobResponse
                {
                    Id = job.Id,
                    Title = job.Title,
                    Company = job.Company,
                    Location = job.Location,
                    Description = job.Description,
                    Salary = job.Salary,
                    Type = job.Type,
                    Industry = job.Industry,
                    ExperienceLevel = job.ExperienceLevel,
                    EducationLevel = job.EducationLevel,
                    IsRemote = job.IsRemote,
                    IsUrgentlyHiring = job.IsUrgentlyHiring,
                    IsSeasonal = job.IsSeasonal,
                    PostedDate = job.PostedDate,
                    UpdatedDate = job.UpdatedDate,
                    IsActive = job.IsActive,
                    EmployerId = job.EmployerId
                };

                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(string id, [FromBody] JobRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
                
                if (job == null)
                {
                    return NotFound($"Job with ID {id} not found");
                }
                var companyName = await GetCompanyNameForEmployerIdAsync(
                    request.EmployerId ?? job.EmployerId
                );

                job.Title = request.Title;
                job.Company = string.IsNullOrWhiteSpace(companyName) ? request.Company : companyName;
                job.Location = request.Location;
                job.Description = request.Description;
                job.Salary = request.Salary;
                job.Type = request.Type;
                job.Industry = request.Industry;
                job.ExperienceLevel = request.ExperienceLevel;
                job.EducationLevel = request.EducationLevel;
                job.IsRemote = request.IsRemote;
                job.IsUrgentlyHiring = request.IsUrgentlyHiring;
                job.IsSeasonal = request.IsSeasonal;
                job.EmployerId = request.EmployerId ?? job.EmployerId;
                job.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(string id)
        {
            try
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
                
                if (job == null)
                {
                    return NotFound($"Job with ID {id} not found");
                }

                // Soft delete
                job.IsActive = false;
                job.UpdatedDate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleJobStatus(string id)
        {
            try
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
                
                if (job == null)
                {
                    return NotFound($"Job with ID {id} not found");
                }

                // Toggle the active status
                job.IsActive = !job.IsActive;
                job.UpdatedDate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = job.IsActive ? "Job activated successfully" : "Job suspended successfully",
                    isActive = job.IsActive 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        private async Task<Dictionary<string, string>> GetCompanyNamesByEmployerIdAsync(
            IEnumerable<Job> jobs
        )
        {
            var employerIds = jobs
                .Where(j => !string.IsNullOrWhiteSpace(j.EmployerId))
                .Select(j => j.EmployerId!)
                .Distinct()
                .ToList();

            if (employerIds.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            return await _context.Users
                .Include(u => u.Company)
                .Where(u => employerIds.Contains(u.Id) && u.Company != null)
                .ToDictionaryAsync(u => u.Id, u => u.Company!.Name);
        }

        private async Task<string?> GetCompanyNameForEmployerIdAsync(string? employerId)
        {
            if (string.IsNullOrWhiteSpace(employerId))
            {
                return null;
            }

            return await _context.Users
                .Include(u => u.Company)
                .Where(u => u.Id == employerId && u.Company != null)
                .Select(u => u.Company!.Name)
                .FirstOrDefaultAsync();
        }

        private static string GetDisplayCompanyName(
            Job job,
            IReadOnlyDictionary<string, string> companyNamesByEmployerId
        )
        {
            if (
                !string.IsNullOrWhiteSpace(job.EmployerId) &&
                companyNamesByEmployerId.TryGetValue(job.EmployerId!, out var companyName) &&
                !string.IsNullOrWhiteSpace(companyName)
            )
            {
                return companyName;
            }

            return job.Company;
        }

        [HttpGet("locations")]
        public async Task<ActionResult<IEnumerable<string>>> GetLocations()
        {
            try
            {
                var locations = await _context.Jobs
                    .Where(j => j.IsActive)
                    .Select(j => j.Location)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToListAsync();

                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetJobTypes()
        {
            try
            {
                var types = await _context.Jobs
                    .Where(j => j.IsActive)
                    .Select(j => j.Type)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToListAsync();

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred. Please try again.");
            }
        }
    }
}
