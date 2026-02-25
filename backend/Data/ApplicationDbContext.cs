using Microsoft.EntityFrameworkCore;
using RightFitGigs.Models;

namespace RightFitGigs.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AppMetric> AppMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Job entity
            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.Location);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.PostedDate);
                
                entity.HasOne(j => j.CompanyNavigation)
                    .WithMany(c => c.Jobs)
                    .HasForeignKey(j => j.CompanyId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Company entity
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Location);
                entity.HasIndex(e => e.Industry);
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserType);
                entity.HasIndex(e => e.Location);
                
                entity.HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Message entity
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.ReceiverId);
                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.SentDate);
                entity.HasIndex(e => e.IsRead);
                
                entity.HasOne(m => m.Job)
                    .WithMany()
                    .HasForeignKey(m => m.JobId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Configure Advertisement entity
            modelBuilder.Entity<Advertisement>(entity =>
            {
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.Platform);
                entity.HasIndex(e => e.Placement);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.StartDate);
            });

            // Configure AppMetric entity
            modelBuilder.Entity<AppMetric>(entity =>
            {
                entity.HasIndex(e => e.MetricType);
                entity.HasIndex(e => e.Platform);
                entity.HasIndex(e => e.CreatedDate);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed sample companies - using fixed IDs
            var techCompanyId = "company-1-tech";
            var mobileCompanyId = "company-2-mobile";
            var designStudioId = "company-3-design";

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = techCompanyId,
                    Name = "Tech Corp",
                    Description = "Leading technology company specializing in mobile and web solutions",
                    Location = "San Francisco",
                    Industry = "Technology",
                    Size = "100-500",
                    Website = "https://techcorp.com",
                    Email = "hr@techcorp.com"
                },
                new Company
                {
                    Id = mobileCompanyId,
                    Name = "Mobile Solutions",
                    Description = "Mobile-first development company",
                    Location = "Remote",
                    Industry = "Technology",
                    Size = "50-100",
                    Website = "https://mobilesolutions.com",
                    Email = "careers@mobilesolutions.com"
                },
                new Company
                {
                    Id = designStudioId,
                    Name = "Design Studio",
                    Description = "Creative design agency for digital products",
                    Location = "New York",
                    Industry = "Design",
                    Size = "10-50",
                    Website = "https://designstudio.com",
                    Email = "jobs@designstudio.com"
                }
            );

            // Seed sample jobs
            modelBuilder.Entity<Job>().HasData(
                new Job
                {
                    Id = "job-1-flutter",
                    Title = "Flutter Developer",
                    Company = "Tech Corp",
                    CompanyId = techCompanyId,
                    Location = "Remote",
                    Description = "Looking for an experienced Flutter developer to join our team.",
                    Salary = "$80k - $120k",
                    Type = "Full-time",
                    PostedDate = new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new Job
                {
                    Id = "job-2-ios",
                    Title = "iOS Developer",
                    Company = "Mobile Solutions",
                    CompanyId = mobileCompanyId,
                    Location = "San Francisco",
                    Description = "Native iOS development position with competitive benefits.",
                    Salary = "$90k - $130k",
                    Type = "Full-time",
                    PostedDate = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc)
                },
                new Job
                {
                    Id = "job-3-designer",
                    Title = "UI/UX Designer",
                    Company = "Design Studio",
                    CompanyId = designStudioId,
                    Location = "New York",
                    Description = "Creative designer needed for mobile and web applications.",
                    Salary = "$70k - $100k",
                    Type = "Contract",
                    PostedDate = new DateTime(2026, 2, 4, 0, 0, 0, DateTimeKind.Utc)
                },
                new Job
                {
                    Id = "job-4-senior-flutter",
                    Title = "Senior Flutter Developer",
                    Company = "Tech Corp",
                    CompanyId = techCompanyId,
                    Location = "Remote",
                    Description = "We are looking for an experienced Flutter developer with 5+ years of experience.",
                    Salary = "$100k - $140k",
                    Type = "Full-time",
                    PostedDate = new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed sample users
            // Note: Password is "password123" for demo user (Admin user)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "user-1-john",
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    Phone = "555-0101",
                    Location = "San Francisco",
                    Bio = "Experienced Flutter developer with passion for mobile development",
                    Skills = "Flutter, Dart, iOS, Android, Firebase",
                    Title = "Senior Flutter Developer",
                    UserType = "Worker",
                    PasswordHash = "$2a$11$eXUE2pctmN2lvrGRJfJiTOLfL02cUcjuY2tnsjG./iopZ6GafngO6", // password123
                    IsAdmin = true // Admin user for dashboard access
                }
            );
        }
    }
}