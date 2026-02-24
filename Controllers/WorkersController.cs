using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.DTOs;
using RightFitGigs.Models;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllWorkers([FromQuery] string? search = null, [FromQuery] string? location = null)
        {
            try
            {
                var query = _context.Users.Where(u => u.IsActive && u.UserType == "Worker").AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchTerm = search.ToLower();
                    query = query.Where(u => 
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm) ||
                        u.Title.ToLower().Contains(searchTerm) ||
                        u.Skills.ToLower().Contains(searchTerm) ||
                        u.Bio.ToLower().Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(location) && location != "All Locations")
                {
                    query = query.Where(u => u.Location == location);
                }

                var workers = await query
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync();

                var response = workers.Select(w => new UserResponse
                {
                    Id = w.Id,
                    FirstName = w.FirstName,
                    LastName = w.LastName,
                    Email = w.Email,
                    Phone = w.Phone,
                    Location = w.Location,
                    Bio = w.Bio,
                    Skills = w.Skills,
                    Title = w.Title,
                    UserType = w.UserType,
                    Initials = w.Initials,
                    CreatedDate = w.CreatedDate,
                    UpdatedDate = w.UpdatedDate,
                    IsActive = w.IsActive
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetWorker(string id)
        {
            try
            {
                var worker = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive && u.UserType == "Worker");
                
                if (worker == null)
                {
                    return NotFound($"Worker with ID {id} not found");
                }

                var response = new UserResponse
                {
                    Id = worker.Id,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    Email = worker.Email,
                    Phone = worker.Phone,
                    Location = worker.Location,
                    Bio = worker.Bio,
                    Skills = worker.Skills,
                    Title = worker.Title,
                    UserType = worker.UserType,
                    Initials = worker.Initials,
                    CreatedDate = worker.CreatedDate,
                    UpdatedDate = worker.UpdatedDate,
                    IsActive = worker.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateWorker([FromBody] UserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if user with email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return Conflict($"User with email {request.Email} already exists");
                }

                var worker = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Location = request.Location,
                    Bio = request.Bio,
                    Skills = request.Skills,
                    Title = request.Title,
                    UserType = "Worker",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.Users.Add(worker);
                await _context.SaveChangesAsync();

                var response = new UserResponse
                {
                    Id = worker.Id,
                    FirstName = worker.FirstName,
                    LastName = worker.LastName,
                    Email = worker.Email,
                    Phone = worker.Phone,
                    Location = worker.Location,
                    Bio = worker.Bio,
                    Skills = worker.Skills,
                    Title = worker.Title,
                    UserType = worker.UserType,
                    Initials = worker.Initials,
                    CreatedDate = worker.CreatedDate,
                    UpdatedDate = worker.UpdatedDate,
                    IsActive = worker.IsActive
                };

                return CreatedAtAction(nameof(GetWorker), new { id = worker.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker(string id, [FromBody] UserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var worker = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.UserType == "Worker");
                
                if (worker == null)
                {
                    return NotFound($"Worker with ID {id} not found");
                }

                // Check if email is being changed and if new email already exists
                if (worker.Email != request.Email)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id);
                    if (existingUser != null)
                    {
                        return Conflict($"User with email {request.Email} already exists");
                    }
                }

                worker.FirstName = request.FirstName;
                worker.LastName = request.LastName;
                worker.Email = request.Email;
                worker.Phone = request.Phone;
                worker.Location = request.Location;
                worker.Bio = request.Bio;
                worker.Skills = request.Skills;
                worker.Title = request.Title;
                worker.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(string id)
        {
            try
            {
                var worker = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.UserType == "Worker");
                
                if (worker == null)
                {
                    return NotFound($"Worker with ID {id} not found");
                }

                // Soft delete
                worker.IsActive = false;
                worker.UpdatedDate = DateTime.UtcNow;
                
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