using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly HashSet<string> AllowedVisitPlatforms = new(StringComparer.OrdinalIgnoreCase)
        {
            "Web", "Android", "Apple", "iOS", "Unknown"
        };

        private static readonly HashSet<string> AllowedDownloadPlatforms = new(StringComparer.OrdinalIgnoreCase)
        {
            "Android", "Apple", "iOS"
        };

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetPlatformStats()
        {
            try
            {
                var activeJobs = await _context.Jobs.CountAsync(j => j.IsActive);
                var totalCandidates = await _context.Users.CountAsync(u => u.IsActive && u.UserType == "Worker");
                var totalCompanies = await _context.Companies.CountAsync(c => c.IsActive);

                var stats = new
                {
                    ActiveJobs = activeJobs,
                    TotalCandidates = totalCandidates,
                    TotalCompanies = totalCompanies
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("recent-activity")]
        public async Task<ActionResult<object>> GetRecentActivity()
        {
            try
            {
                // Get recent jobs (last 7 days)
                var recentJobs = await _context.Jobs
                    .Where(j => j.IsActive && j.PostedDate >= DateTime.UtcNow.AddDays(-7))
                    .OrderByDescending(j => j.PostedDate)
                    .Take(3)
                    .Select(j => new
                    {
                        Type = "job_posted",
                        Title = $"New job posted: {j.Title}",
                        Subtitle = j.Company,
                        Time = GetTimeAgo(j.PostedDate),
                        Icon = "add_circle_outline"
                    })
                    .ToListAsync();

                // Get recent users (last 7 days)
                var recentUsers = await _context.Users
                    .Where(u => u.IsActive && u.UserType == "Worker" && u.CreatedDate >= DateTime.UtcNow.AddDays(-7))
                    .OrderByDescending(u => u.CreatedDate)
                    .Take(2)
                    .Select(u => new
                    {
                        Type = "profile_updated",
                        Title = $"Profile updated: {u.Title} available",
                        Subtitle = $"{u.FirstName} {u.LastName}",
                        Time = GetTimeAgo(u.CreatedDate),
                        Icon = "person_outline"
                    })
                    .ToListAsync();

                // Get recent companies (last 7 days)
                var recentCompanies = await _context.Companies
                    .Where(c => c.IsActive && c.CreatedDate >= DateTime.UtcNow.AddDays(-7))
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(2)
                    .Select(c => new
                    {
                        Type = "company_joined",
                        Title = $"New company joined: {c.Name}",
                        Subtitle = c.Name,
                        Time = GetTimeAgo(c.CreatedDate),
                        Icon = "business_outlined"
                    })
                    .ToListAsync();

                var allActivity = new List<object>();
                allActivity.AddRange(recentJobs);
                allActivity.AddRange(recentUsers);
                allActivity.AddRange(recentCompanies);

                // Sort by time (most recent first) and take top 5
                var sortedActivity = allActivity
                    .OrderByDescending(a => GetDateFromTimeAgo(((dynamic)a).Time))
                    .Take(5)
                    .ToList();

                return Ok(sortedActivity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("track-visit")]
        public async Task<IActionResult> TrackVisit([FromBody] TrackMetricRequest? request)
        {
            try
            {
                var platform = NormalizeVisitPlatform(request?.Platform);

                var metric = new Models.AppMetric
                {
                    MetricType = "Visit",
                    Platform = platform,
                    CreatedDate = DateTime.UtcNow
                };

                _context.AppMetrics.Add(metric);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Visit tracked" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("track-download")]
        public async Task<IActionResult> TrackDownload([FromBody] TrackMetricRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Platform))
                {
                    return BadRequest(new { message = "Platform is required. Use Apple or Android." });
                }

                var normalizedPlatform = NormalizeDownloadPlatform(request.Platform);
                if (!AllowedDownloadPlatforms.Contains(normalizedPlatform))
                {
                    return BadRequest(new { message = "Invalid platform. Use Apple or Android." });
                }

                var metric = new Models.AppMetric
                {
                    MetricType = "Download",
                    Platform = normalizedPlatform,
                    CreatedDate = DateTime.UtcNow
                };

                _context.AppMetrics.Add(metric);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Download tracked" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string NormalizeVisitPlatform(string? platform)
        {
            if (string.IsNullOrWhiteSpace(platform))
                return "Unknown";

            var normalized = NormalizePlatform(platform);
            return AllowedVisitPlatforms.Contains(normalized) ? normalized : "Unknown";
        }

        private string NormalizeDownloadPlatform(string platform)
        {
            return NormalizePlatform(platform);
        }

        private string NormalizePlatform(string platform)
        {
            return platform.Trim().Equals("ios", StringComparison.OrdinalIgnoreCase)
                ? "Apple"
                : platform.Trim();
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
            else
                return "Just now";
        }

        private DateTime GetDateFromTimeAgo(string timeAgo)
        {
            // This is a simple way to sort - in a real app you'd store the actual datetime
            if (timeAgo.Contains("day"))
            {
                var days = int.Parse(timeAgo.Split(' ')[0]);
                return DateTime.UtcNow.AddDays(-days);
            }
            else if (timeAgo.Contains("hour"))
            {
                var hours = int.Parse(timeAgo.Split(' ')[0]);
                return DateTime.UtcNow.AddHours(-hours);
            }
            else if (timeAgo.Contains("minute"))
            {
                var minutes = int.Parse(timeAgo.Split(' ')[0]);
                return DateTime.UtcNow.AddMinutes(-minutes);
            }
            else
            {
                return DateTime.UtcNow;
            }
        }
    }

    public class TrackMetricRequest
    {
        public string? Platform { get; set; }
    }
}