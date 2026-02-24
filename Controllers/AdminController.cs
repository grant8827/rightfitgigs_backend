using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightFitGigs.Data;

namespace RightFitGigs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/admin/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var totalWorkers = await _context.Users
                    .Where(u => u.UserType.ToLower() == "worker")
                    .CountAsync();

                var totalEmployers = await _context.Users
                    .Where(u => u.UserType.ToLower() == "employer")
                    .CountAsync();

                var totalJobs = await _context.Jobs.CountAsync();
                var activeJobs = await _context.Jobs
                    .Where(j => j.IsActive)
                    .CountAsync();

                var totalApplications = await _context.Applications.CountAsync();
                var pendingApplications = await _context.Applications
                    .Where(a => a.Status == "Pending")
                    .CountAsync();

                var totalCompanies = await _context.Companies.CountAsync();
                var totalMessages = await _context.Messages.CountAsync();
                var totalNotifications = await _context.Notifications.CountAsync();
                var totalAds = await _context.Advertisements.CountAsync();
                var activeAds = await _context.Advertisements
                    .Where(a => a.IsActive)
                    .CountAsync();

                var utcNow = DateTime.UtcNow;
                var startOfToday = utcNow.Date;
                var startOfMonth = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

                var totalVisits = await _context.AppMetrics
                    .Where(m => m.MetricType == "Visit")
                    .CountAsync();

                var visitsToday = await _context.AppMetrics
                    .Where(m => m.MetricType == "Visit" && m.CreatedDate >= startOfToday)
                    .CountAsync();

                var visitsThisMonth = await _context.AppMetrics
                    .Where(m => m.MetricType == "Visit" && m.CreatedDate >= startOfMonth)
                    .CountAsync();

                var appleDownloads = await _context.AppMetrics
                    .Where(m => m.MetricType == "Download" && (m.Platform == "Apple" || m.Platform == "iOS"))
                    .CountAsync();

                var androidDownloads = await _context.AppMetrics
                    .Where(m => m.MetricType == "Download" && m.Platform == "Android")
                    .CountAsync();

                // Recent activity
                var recentJobs = await _context.Jobs
                    .OrderByDescending(j => j.PostedDate)
                    .Take(5)
                    .Select(j => new
                    {
                        j.Id,
                        j.Title,
                        j.Company,
                        j.Location,
                        j.PostedDate
                    })
                    .ToListAsync();

                var recentApplications = await _context.Applications
                    .OrderByDescending(a => a.AppliedDate)
                    .Take(5)
                    .Include(a => a.Job)
                    .Select(a => new
                    {
                        a.Id,
                        a.JobId,
                        JobTitle = a.Job != null ? a.Job.Title : "N/A",
                        a.WorkerName,
                        a.Status,
                        a.AppliedDate
                    })
                    .ToListAsync();

                // Job type distribution
                var jobTypeStats = await _context.Jobs
                    .GroupBy(j => j.Type)
                    .Select(g => new
                    {
                        Type = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Application status distribution
                var applicationStatusStats = await _context.Applications
                    .GroupBy(a => a.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Monthly job postings (last 6 months)
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
                var monthlyJobStats = await _context.Jobs
                    .Where(j => j.PostedDate >= sixMonthsAgo)
                    .GroupBy(j => new { j.PostedDate.Year, j.PostedDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // Monthly user registrations (last 6 months)
                var monthlyUserStats = await _context.Users
                    .Where(u => u.CreatedDate >= sixMonthsAgo)
                    .GroupBy(u => new { u.CreatedDate.Year, u.CreatedDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Workers = g.Count(u => u.UserType.ToLower() == "worker"),
                        Employers = g.Count(u => u.UserType.ToLower() == "employer"),
                        Total = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // Monthly application trends (last 6 months)
                var monthlyApplicationStats = await _context.Applications
                    .Where(a => a.AppliedDate >= sixMonthsAgo)
                    .GroupBy(a => new { a.AppliedDate.Year, a.AppliedDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // Daily visits (last 30 days)
                var thirtyDaysAgo = utcNow.AddDays(-30);
                var dailyVisitStats = await _context.AppMetrics
                    .Where(m => m.MetricType == "Visit" && m.CreatedDate >= thirtyDaysAgo)
                    .GroupBy(m => m.CreatedDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                // Monthly visits (last 12 months)
                var twelveMonthsAgo = utcNow.AddMonths(-12);
                var monthlyVisitStats = await _context.AppMetrics
                    .Where(m => m.MetricType == "Visit" && m.CreatedDate >= twelveMonthsAgo)
                    .GroupBy(m => new { m.CreatedDate.Year, m.CreatedDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // Recent activity feed (last 20 activities)
                var recentMessages = await _context.Messages
                    .OrderByDescending(m => m.SentDate)
                    .Take(10)
                    .Select(m => new
                    {
                        Type = "Message",
                        m.Id,
                        Description = $"{m.SenderName} sent a message",
                        Timestamp = m.SentDate
                    })
                    .ToListAsync();

                var recentNotifications = await _context.Notifications
                    .OrderByDescending(n => n.CreatedDate)
                    .Take(10)
                    .Select(n => new
                    {
                        Type = "Notification",
                        n.Id,
                        Description = n.Message ?? "",
                        Timestamp = n.CreatedDate
                    })
                    .ToListAsync();

                // Combine all activities and sort by timestamp
                var jobActivities = recentJobs.Select(j => new
                {
                    Type = "Job",
                    j.Id,
                    Description = $"New job posted: {j.Title} at {j.Company}",
                    Timestamp = j.PostedDate
                }).ToList();

                var applicationActivities = recentApplications.Select(a => new
                {
                    Type = "Application",
                    a.Id,
                    Description = $"{a.WorkerName} applied to {a.JobTitle}",
                    Timestamp = a.AppliedDate
                }).ToList();

                var activityFeed = jobActivities
                    .Concat(applicationActivities)
                    .Concat(recentMessages)
                    .Concat(recentNotifications)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(20)
                    .ToList();

                return Ok(new
                {
                    overview = new
                    {
                        totalWorkers,
                        totalEmployers,
                        totalUsers = totalWorkers + totalEmployers,
                        totalJobs,
                        activeJobs,
                        totalApplications,
                        pendingApplications,
                        totalCompanies,
                        totalMessages,
                        totalNotifications,
                        totalAds,
                        activeAds,
                        visitsToday,
                        visitsThisMonth,
                        totalVisits,
                        appleDownloads,
                        androidDownloads,
                        totalDownloads = appleDownloads + androidDownloads
                    },
                    recentActivity = new
                    {
                        recentJobs,
                        recentApplications,
                        activityFeed
                    },
                    analytics = new
                    {
                        jobTypeStats,
                        applicationStatusStats,
                        monthlyJobStats,
                        monthlyUserStats,
                        monthlyApplicationStats,
                        dailyVisitStats,
                        monthlyVisitStats
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard stats");
                return StatusCode(500, new { message = "Error fetching dashboard stats" });
            }
        }

        // GET: api/admin/users/workers
        [HttpGet("users/workers")]
        public async Task<ActionResult<object>> GetWorkers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users.Where(u => u.UserType.ToLower() == "worker");
                var total = await query.CountAsync();

                var workers = await query
                    .OrderByDescending(u => u.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Phone,
                        u.Location,
                        u.Title,
                        u.Skills,
                        u.DesiredJobTitle,
                        u.DesiredLocation,
                        u.DesiredSalaryRange,
                        u.CreatedDate,
                        u.IsActive
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = workers,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching workers");
                return StatusCode(500, new { message = "Error fetching workers" });
            }
        }

        // GET: api/admin/users/employers
        [HttpGet("users/employers")]
        public async Task<ActionResult<object>> GetEmployers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users.Where(u => u.UserType.ToLower() == "employer");
                var total = await query.CountAsync();

                var employers = await query
                    .OrderByDescending(u => u.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.Company)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Phone,
                        u.Location,
                        u.Title,
                        CompanyName = u.Company != null ? u.Company.Name : "N/A",
                        u.CompanyId,
                        u.CreatedDate,
                        u.IsActive
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = employers,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employers");
                return StatusCode(500, new { message = "Error fetching employers" });
            }
        }

        // GET: api/admin/jobs/all
        [HttpGet("jobs/all")]
        public async Task<ActionResult<object>> GetAllJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Jobs.AsQueryable();
                var total = await query.CountAsync();

                var jobs = await query
                    .OrderByDescending(j => j.PostedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(j => new
                    {
                        j.Id,
                        j.Title,
                        j.Company,
                        j.Location,
                        j.Type,
                        j.Salary,
                        j.ExperienceLevel,
                        j.PostedDate,
                        j.IsActive,
                        ApplicationCount = _context.Applications.Count(a => a.JobId == j.Id)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = jobs,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs");
                return StatusCode(500, new { message = "Error fetching jobs" });
            }
        }

        // GET: api/admin/applications/all
        [HttpGet("applications/all")]
        public async Task<ActionResult<object>> GetAllApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Applications.AsQueryable();
                var total = await query.CountAsync();

                var applications = await query
                    .OrderByDescending(a => a.AppliedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(a => a.Job)
                    .Select(a => new
                    {
                        a.Id,
                        a.JobId,
                        JobTitle = a.Job != null ? a.Job.Title : "N/A",
                        Company = a.Job != null ? a.Job.Company : "N/A",
                        a.WorkerId,
                        a.WorkerName,
                        a.WorkerEmail,
                        a.WorkerPhone,
                        a.Status,
                        a.CoverLetter,
                        a.AppliedDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = applications,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching applications");
                return StatusCode(500, new { message = "Error fetching applications" });
            }
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, new { message = "Error deleting user" });
            }
        }

        // DELETE: api/admin/jobs/{id}
        [HttpDelete("jobs/{id}")]
        public async Task<IActionResult> DeleteJob(string id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    return NotFound(new { message = "Job not found" });
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Job deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job");
                return StatusCode(500, new { message = "Error deleting job" });
            }
        }

        // PATCH: api/admin/users/{id}/toggle-active
        [HttpPatch("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                user.IsActive = !user.IsActive;
                user.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User status updated", isActive = user.IsActive });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status");
                return StatusCode(500, new { message = "Error updating user status" });
            }
        }

        // PATCH: api/admin/jobs/{id}/toggle-active
        [HttpPatch("jobs/{id}/toggle-active")]
        public async Task<IActionResult> ToggleJobActive(string id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    return NotFound(new { message = "Job not found" });
                }

                job.IsActive = !job.IsActive;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Job status updated", isActive = job.IsActive });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling job status");
                return StatusCode(500, new { message = "Error updating job status" });
            }
        }
    }
}
