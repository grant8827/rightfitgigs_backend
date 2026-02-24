using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.FileProviders;
using Npgsql;
using RightFitGigs.Data;
using RightFitGigs.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var frontendUrl = builder.Configuration["FRONTEND_URL"];
var defaultFrontendUrl = "https://rightfitgigsfrontendr.up.railway.app";
var allowedOrigins = new[]
{
    frontendUrl,
    defaultFrontendUrl,
    "https://rightfitgigs.com",
    "https://www.rightfitgigs.com",
    "http://localhost:5173",
    "http://127.0.0.1:5173"
}
.Where(origin => !string.IsNullOrWhiteSpace(origin))
.Select(origin => origin!.Trim().TrimEnd('/'))
.Distinct(StringComparer.OrdinalIgnoreCase)
.ToArray();

// Add Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=hopper.proxy.rlwy.net;Port=33137;Database=railway;Username=postgres;Password=bKnpjKnCKoqpuWyTjTziZxfNtceZRXIs";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  // Check if the origin is in the configured list
                  var isAllowed = allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);

                  // Also allow any localhost origin for local development (e.g., Flutter web)
                  if (!isAllowed && (origin.StartsWith("http://localhost:") || origin.StartsWith("http://127.0.0.1:")))
                  {
                      isAllowed = true;
                  }
                  return isAllowed;
              })
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
    var connection = context.Database.GetDbConnection();
    if (connection.State != System.Data.ConnectionState.Open)
    {
        connection.Open();
    }

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT to_regclass('public.\"Users\"') IS NOT NULL";
    var usersTableExists = Convert.ToBoolean(command.ExecuteScalar() ?? false);

    if (!usersTableExists)
    {
        var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
        databaseCreator.CreateTables();
    }
    else
    {
        // Fix DateTime columns if they are still TEXT type
        try
        {
            var fixCommand = connection.CreateCommand();
            fixCommand.CommandText = @"
                DO $$ 
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Users' AND column_name = 'CreatedDate' AND data_type = 'text') THEN
                        ALTER TABLE ""Users"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone USING ""CreatedDate""::timestamp with time zone;
                        ALTER TABLE ""Users"" ALTER COLUMN ""UpdatedDate"" TYPE timestamp with time zone USING ""UpdatedDate""::timestamp with time zone;
                        ALTER TABLE ""Companies"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone USING ""CreatedDate""::timestamp with time zone;
                        ALTER TABLE ""Companies"" ALTER COLUMN ""UpdatedDate"" TYPE timestamp with time zone USING ""UpdatedDate""::timestamp with time zone;
                        ALTER TABLE ""Jobs"" ALTER COLUMN ""PostedDate"" TYPE timestamp with time zone USING ""PostedDate""::timestamp with time zone;
                        ALTER TABLE ""Jobs"" ALTER COLUMN ""UpdatedDate"" TYPE timestamp with time zone USING ""UpdatedDate""::timestamp with time zone;
                        ALTER TABLE ""Messages"" ALTER COLUMN ""SentDate"" TYPE timestamp with time zone USING ""SentDate""::timestamp with time zone;
                        ALTER TABLE ""Messages"" ALTER COLUMN ""ReadDate"" TYPE timestamp with time zone USING ""ReadDate""::timestamp with time zone;
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Applications') THEN
                            ALTER TABLE ""Applications"" ALTER COLUMN ""AppliedDate"" TYPE timestamp with time zone USING ""AppliedDate""::timestamp with time zone;
                            ALTER TABLE ""Applications"" ALTER COLUMN ""UpdatedDate"" TYPE timestamp with time zone USING ""UpdatedDate""::timestamp with time zone;
                        END IF;
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Notifications') THEN
                            ALTER TABLE ""Notifications"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone USING ""CreatedDate""::timestamp with time zone;
                            ALTER TABLE ""Notifications"" ALTER COLUMN ""ReadDate"" TYPE timestamp with time zone USING ""ReadDate""::timestamp with time zone;
                        END IF;
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Advertisements') THEN
                            ALTER TABLE ""Advertisements"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone USING ""CreatedDate""::timestamp with time zone;
                            ALTER TABLE ""Advertisements"" ALTER COLUMN ""UpdatedDate"" TYPE timestamp with time zone USING ""UpdatedDate""::timestamp with time zone;
                            ALTER TABLE ""Advertisements"" ALTER COLUMN ""StartDate"" TYPE timestamp with time zone USING ""StartDate""::timestamp with time zone;
                            ALTER TABLE ""Advertisements"" ALTER COLUMN ""EndDate"" TYPE timestamp with time zone USING ""EndDate""::timestamp with time zone;
                        END IF;
                        IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'AppMetrics') THEN
                            ALTER TABLE ""AppMetrics"" ALTER COLUMN ""CreatedDate"" TYPE timestamp with time zone USING ""CreatedDate""::timestamp with time zone;
                        END IF;
                    END IF;
                END $$;";
            fixCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not fix DateTime columns: {ex.Message}");
        }
    }

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
