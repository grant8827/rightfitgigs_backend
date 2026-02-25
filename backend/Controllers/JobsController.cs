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
                // Return all jobs (both active and suspended)
                // Let the frontend decide which ones to display
                var query = _context.Jobs.AsQueryable();

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

                // Order by posted date (newest first)
                query = query.OrderByDescending(j => j.PostedDate);

                // Apply pagination
                var totalCount = await query.CountAsync();
                var jobs = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var response = jobs.Select(j => new JobResponse
                {
                    Id = j.Id,
                    Title = j.Title,
                    Company = j.Company,
                    Location = j.Location,
                    Description = j.Description,
                    Salary = j.Salary,
                    Type = j.Type,
                    Industry = j.Industry,
                    ExperienceLevel = j.ExperienceLevel,
                    IsRemote = j.IsRemote,
                    IsUrgentlyHiring = j.IsUrgentlyHiring,
                    IsSeasonal = j.IsSeasonal,
                    PostedDate = j.PostedDate,
                    UpdatedDate = j.UpdatedDate,
                    IsActive = j.IsActive
                }).ToList();

                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                    IsRemote = job.IsRemote,
                    IsUrgentlyHiring = job.IsUrgentlyHiring,
                    IsSeasonal = job.IsSeasonal,
                    PostedDate = job.PostedDate,
                    UpdatedDate = job.UpdatedDate,
                    IsActive = job.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                    IsRemote = request.IsRemote,
                    IsUrgentlyHiring = request.IsUrgentlyHiring,
                    IsSeasonal = request.IsSeasonal
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
                    IsRemote = job.IsRemote,
                    IsUrgentlyHiring = job.IsUrgentlyHiring,
                    IsSeasonal = job.IsSeasonal,
                    PostedDate = job.PostedDate,
                    UpdatedDate = job.UpdatedDate,
                    IsActive = job.IsActive
                };

                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

                job.Title = request.Title;
                job.Company = request.Company;
                job.Location = request.Location;
                job.Description = request.Description;
                job.Salary = request.Salary;
                job.Type = request.Type;
                job.Industry = request.Industry;
                job.ExperienceLevel = request.ExperienceLevel;
                job.IsRemote = request.IsRemote;
                job.IsUrgentlyHiring = request.IsUrgentlyHiring;
                job.IsSeasonal = request.IsSeasonal;
                job.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}