using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RightFitGigs.Data;
using RightFitGigs.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=rightfitgigs.db")
           .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Enable static file serving

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var testEmployerEmail = "employer.test@example.com";
    var existingEmployer = context.Users.FirstOrDefault(u => u.Email == testEmployerEmail);

    if (existingEmployer == null)
    {
        var company = context.Companies.FirstOrDefault(c => c.Email == testEmployerEmail);
        if (company == null)
        {
            company = new Company
            {
                Name = "Test Employer Company",
                Description = "Seeded company for admin employer tab testing",
                Location = "Remote",
                Industry = "Technology",
                Size = "10-50",
                Website = "https://example.com",
                Email = testEmployerEmail,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            context.Companies.Add(company);
            context.SaveChanges();
        }

        var employerUser = new User
        {
            FirstName = "Test",
            LastName = "Employer",
            Email = testEmployerEmail,
            Phone = "555-0202",
            Location = "Remote",
            Title = "Hiring Manager",
            UserType = "Employer",
            CompanyId = company.Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true,
            IsAdmin = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        context.Users.Add(employerUser);
        context.SaveChanges();
    }
}

app.Run();
