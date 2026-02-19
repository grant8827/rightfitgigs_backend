using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;
using RightFitGigs.Models;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdvertisementsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AdvertisementsController(
            ApplicationDbContext context, 
            ILogger<AdvertisementsController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: api/advertisements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisements(
            [FromQuery] string? platform = null,
            [FromQuery] bool? activeOnly = null,
            [FromQuery] string? placement = null)
        {
            try
            {
                var query = _context.Advertisements.AsQueryable();

                if (!string.IsNullOrEmpty(platform) && platform != "Both")
                {
                    query = query.Where(a => a.Platform == platform || a.Platform == "Both");
                }

                if (!string.IsNullOrWhiteSpace(placement))
                {
                    query = query.Where(a => a.Placement == placement);
                }

                if (activeOnly == true)
                {
                    var now = DateTime.UtcNow;
                    query = query.Where(a => 
                        a.IsActive && 
                        a.StartDate <= now && 
                        (a.EndDate == null || a.EndDate >= now));
                }

                var ads = await query
                    .OrderBy(a => a.DisplayOrder)
                    .ThenByDescending(a => a.CreatedDate)
                    .ToListAsync();

                return Ok(ads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching advertisements");
                return StatusCode(500, new { message = "Error fetching advertisements" });
            }
        }

        // GET: api/advertisements/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisement(string id)
        {
            try
            {
                var ad = await _context.Advertisements.FindAsync(id);
                if (ad == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                return Ok(ad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching advertisement");
                return StatusCode(500, new { message = "Error fetching advertisement" });
            }
        }

        // POST: api/advertisements
        [HttpPost]
        public async Task<ActionResult<Advertisement>> CreateAdvertisement([FromForm] AdvertisementCreateDto dto)
        {
            try
            {
                // Validate file type
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var allowedVideoExtensions = new[] { ".mp4", ".webm", ".mov", ".avi" };
                var adType = "Image";
                var fileUrl = string.Empty;
                string? originalFileName = null;

                if (dto.File != null && dto.File.Length > 0)
                {
                    var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

                    if (allowedImageExtensions.Contains(fileExtension))
                    {
                        adType = "Image";
                    }
                    else if (allowedVideoExtensions.Contains(fileExtension))
                    {
                        adType = "Video";
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid file type. Please upload an image or video file." });
                    }

                    var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", "advertisements");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    fileUrl = $"/uploads/advertisements/{fileName}";
                    originalFileName = dto.File.FileName;
                }

                // Create advertisement record
                var startDate = dto.StartDate ?? DateTime.UtcNow;

                if (dto.EndDate.HasValue && dto.EndDate.Value <= startDate)
                {
                    return BadRequest(new { message = "End date must be later than start date." });
                }

                var advertisement = new Advertisement
                {
                    Title = dto.Title?.Trim() ?? string.Empty,
                    Description = dto.Description?.Trim() ?? string.Empty,
                    Type = adType,
                    FileUrl = fileUrl,
                    FileName = originalFileName,
                    Platform = dto.Platform ?? "Both",
                    Placement = dto.Placement ?? "Popup",
                    Position = dto.Position ?? "BottomRight",
                    FadeDurationSeconds = dto.FadeDurationSeconds ?? 8,
                    IsDismissible = dto.IsDismissible ?? true,
                    TargetUrl = dto.TargetUrl,
                    BusinessName = dto.BusinessName,
                    DisplayOrder = dto.DisplayOrder ?? 0,
                    IsActive = dto.IsActive ?? true,
                    StartDate = startDate,
                    EndDate = dto.EndDate,
                    CreatedBy = dto.CreatedBy ?? "Admin",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _context.Advertisements.Add(advertisement);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAdvertisement), new { id = advertisement.Id }, advertisement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating advertisement");
                return StatusCode(500, new { message = "Error creating advertisement" });
            }
        }

        // PUT: api/advertisements/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisement(string id, [FromForm] AdvertisementUpdateDto dto)
        {
            try
            {
                var advertisement = await _context.Advertisements.FindAsync(id);
                if (advertisement == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                // Update fields
                if (dto.Title != null)
                    advertisement.Title = dto.Title.Trim();

                if (dto.Description != null)
                    advertisement.Description = dto.Description.Trim();

                if (!string.IsNullOrEmpty(dto.Platform))
                    advertisement.Platform = dto.Platform;

                if (!string.IsNullOrWhiteSpace(dto.Placement))
                    advertisement.Placement = dto.Placement;

                if (!string.IsNullOrWhiteSpace(dto.Position))
                    advertisement.Position = dto.Position;

                if (dto.FadeDurationSeconds.HasValue)
                    advertisement.FadeDurationSeconds = dto.FadeDurationSeconds.Value;

                if (dto.IsDismissible.HasValue)
                    advertisement.IsDismissible = dto.IsDismissible.Value;

                if (dto.TargetUrl != null)
                    advertisement.TargetUrl = dto.TargetUrl;

                if (dto.BusinessName != null)
                    advertisement.BusinessName = dto.BusinessName;

                if (dto.DisplayOrder.HasValue)
                    advertisement.DisplayOrder = dto.DisplayOrder.Value;

                if (dto.IsActive.HasValue)
                    advertisement.IsActive = dto.IsActive.Value;

                if (dto.StartDate.HasValue)
                    advertisement.StartDate = dto.StartDate.Value;

                if (dto.EndDate != null)
                    advertisement.EndDate = dto.EndDate;

                if (advertisement.EndDate.HasValue && advertisement.EndDate.Value <= advertisement.StartDate)
                {
                    return BadRequest(new { message = "End date must be later than start date." });
                }

                // Handle file update if provided
                if (dto.File != null && dto.File.Length > 0)
                {
                    // Delete old file
                    var oldFilePath = Path.Combine(_environment.ContentRootPath, advertisement.FileUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    // Save new file
                    var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", "advertisements");
                    Directory.CreateDirectory(uploadsPath);

                    var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    advertisement.FileUrl = $"/uploads/advertisements/{fileName}";
                    advertisement.FileName = dto.File.FileName;

                    // Update type based on new file
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var allowedVideoExtensions = new[] { ".mp4", ".webm", ".mov", ".avi" };

                    if (allowedImageExtensions.Contains(fileExtension))
                    {
                        advertisement.Type = "Image";
                    }
                    else if (allowedVideoExtensions.Contains(fileExtension))
                    {
                        advertisement.Type = "Video";
                    }
                }

                advertisement.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(advertisement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating advertisement");
                return StatusCode(500, new { message = "Error updating advertisement" });
            }
        }

        // DELETE: api/advertisements/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(string id)
        {
            try
            {
                var advertisement = await _context.Advertisements.FindAsync(id);
                if (advertisement == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                // Delete file
                var filePath = Path.Combine(_environment.ContentRootPath, advertisement.FileUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Advertisements.Remove(advertisement);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Advertisement deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting advertisement");
                return StatusCode(500, new { message = "Error deleting advertisement" });
            }
        }

        // PATCH: api/advertisements/{id}/toggle-active
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(string id)
        {
            try
            {
                var advertisement = await _context.Advertisements.FindAsync(id);
                if (advertisement == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                advertisement.IsActive = !advertisement.IsActive;
                advertisement.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Advertisement status updated", isActive = advertisement.IsActive });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling advertisement status");
                return StatusCode(500, new { message = "Error updating advertisement status" });
            }
        }

        // POST: api/advertisements/{id}/track-view
        [HttpPost("{id}/track-view")]
        public async Task<IActionResult> TrackView(string id)
        {
            try
            {
                var advertisement = await _context.Advertisements.FindAsync(id);
                if (advertisement == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                advertisement.ViewCount++;
                await _context.SaveChangesAsync();

                return Ok(new { message = "View tracked" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking view");
                return StatusCode(500, new { message = "Error tracking view" });
            }
        }

        // POST: api/advertisements/{id}/track-click
        [HttpPost("{id}/track-click")]
        public async Task<IActionResult> TrackClick(string id)
        {
            try
            {
                var advertisement = await _context.Advertisements.FindAsync(id);
                if (advertisement == null)
                {
                    return NotFound(new { message = "Advertisement not found" });
                }

                advertisement.ClickCount++;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Click tracked" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking click");
                return StatusCode(500, new { message = "Error tracking click" });
            }
        }
    }

    // DTOs for Advertisement operations
    public class AdvertisementCreateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
        public string? Platform { get; set; }
        public string? Placement { get; set; }
        public string? Position { get; set; }
        public int? FadeDurationSeconds { get; set; }
        public bool? IsDismissible { get; set; }
        public string? TargetUrl { get; set; }
        public string? BusinessName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class AdvertisementUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
        public string? Platform { get; set; }
        public string? Placement { get; set; }
        public string? Position { get; set; }
        public int? FadeDurationSeconds { get; set; }
        public bool? IsDismissible { get; set; }
        public string? TargetUrl { get; set; }
        public string? BusinessName { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
