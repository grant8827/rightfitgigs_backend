using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using RightFitGigs.Data;
using RightFitGigs.Models;
using RightFitGigs.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on Railway's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "5071";
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<PendingRegistrationStore>();

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
// Set DATABASE_URL environment variable (Railway) or DefaultConnection in appsettings.json
var rawConnectionString =
    (Environment.GetEnvironmentVariable("DATABASE_URL") is string dbUrl && !string.IsNullOrWhiteSpace(dbUrl) ? dbUrl : null)
    ?? (builder.Configuration.GetConnectionString("DefaultConnection") is string dbConn && !string.IsNullOrWhiteSpace(dbConn) ? dbConn : null)
    ?? throw new InvalidOperationException("No database connection string configured. Set the DATABASE_URL environment variable in Railway.");

// Convert PostgreSQL URI format (postgresql://user:pass@host:port/db) to Npgsql key-value format
static string ConvertPostgresUrl(string url)
{
    if (!url.StartsWith("postgres://") && !url.StartsWith("postgresql://"))
        return url; // already in key-value format

    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':');
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');

    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}

var connectionString = ConvertPostgresUrl(rawConnectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add CORS
var corsOrigins = allowedOrigins;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Total-Count");
    });
});

// Add JWT Authentication
var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? "RFG_Dev_Only_Secret_Key_Must_Be_At_Least_32_Characters_Long!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = "rightfitgigs",
            ValidateAudience = true,
            ValidAudience = "rightfitgigs",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // AdminOnly: token must contain isAdmin=true claim
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("isAdmin", "true"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Don't use HTTPS redirection - Railway handles SSL at load balancer
// app.UseHttpsRedirection();

// CORS must come before static files and routing
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); // Enable static file serving

// Use UPLOADS_PATH env var if set (Railway persistent volume), otherwise fall back to local path
var uploadsPath = Environment.GetEnvironmentVariable("UPLOADS_PATH")
    ?? Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// Also serve resumes subfolder
var resumesPath = Path.Combine(uploadsPath, "resumes");
Directory.CreateDirectory(resumesPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(resumesPath),
    RequestPath = "/uploads/resumes"
});

// Health check endpoint for Railway
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

// Ensure database is created (with error handling to not block startup)
try
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var hasPersistentUploadsPath = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("UPLOADS_PATH"));
    
    logger.LogInformation("Starting database initialization...");
    logger.LogInformation("Uploads path resolved to {UploadsPath}. Persistent volume configured: {HasPersistentUploadsPath}", uploadsPath, hasPersistentUploadsPath);
    if (!hasPersistentUploadsPath)
    {
        logger.LogWarning("UPLOADS_PATH is not configured. Uploaded files will be stored in container-local storage and may be lost after redeploy or restart.");
    }
    
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Set command timeout to 30 seconds
    context.Database.SetCommandTimeout(30);
    
    var connection = context.Database.GetDbConnection();
    if (connection.State != System.Data.ConnectionState.Open)
    {
        connection.Open();
    }

    using var command = connection.CreateCommand();
    command.CommandTimeout = 30;
    command.CommandText = "SELECT to_regclass('public.\"Users\"') IS NOT NULL";
    var usersTableExists = Convert.ToBoolean(command.ExecuteScalar() ?? false);logger.LogInformation("Users table exists: {Exists}", usersTableExists);

    if (!usersTableExists)
    {
        logger.LogInformation("Creating database tables...");
        var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
        databaseCreator.CreateTables();
        logger.LogInformation("Database tables created successfully");
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

        // Add EmployerId column to Jobs table if it doesn't exist
        try
        {
            var addColumnCommand = connection.CreateCommand();
            addColumnCommand.CommandText = @"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Jobs' AND column_name = 'EmployerId') THEN
                        ALTER TABLE ""Jobs"" ADD COLUMN ""EmployerId"" text;
                    END IF;
                END $$;";
            addColumnCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not add EmployerId column: {ex.Message}");
        }

        // Add EducationLevel column to Jobs table if it doesn't exist
        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Jobs' AND column_name = 'EducationLevel') THEN
                        ALTER TABLE ""Jobs"" ADD COLUMN ""EducationLevel"" text;
                    END IF;
                END $$;";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not add EducationLevel to Jobs: {ex.Message}");
        }

        // Add EducationLevel column to Users table if it doesn't exist
        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Users' AND column_name = 'EducationLevel') THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""EducationLevel"" text;
                    END IF;
                END $$;";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not add EducationLevel to Users: {ex.Message}");
        }

        // Add PasswordResetToken and PasswordResetExpiry columns to Users if they don't exist
        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Users' AND column_name = 'PasswordResetToken') THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""PasswordResetToken"" text;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Users' AND column_name = 'PasswordResetExpiry') THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""PasswordResetExpiry"" timestamp with time zone;
                    END IF;
                END $$;";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not add PasswordReset columns to Users: {ex.Message}");
        }

        // Create Job_Preferences table if it doesn't exist
        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ""Job_Preferences"" (
                    ""Id"" text NOT NULL PRIMARY KEY,
                    ""UserId"" text NOT NULL UNIQUE REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                    ""DesiredJobTitle"" character varying(100),
                    ""DesiredLocation"" character varying(200),
                    ""DesiredSalaryRange"" character varying(50),
                    ""DesiredJobType"" character varying(50),
                    ""DesiredExperienceLevel"" character varying(50),
                    ""OpenToRemote"" boolean NOT NULL DEFAULT true,
                    ""PreferredIndustries"" character varying(200),
                    ""EducationLevel"" character varying(50),
                    ""UpdatedDate"" timestamp with time zone NOT NULL DEFAULT now()
                );";
            cmd.ExecuteNonQuery();
            logger.LogInformation("Job_Preferences table ensured.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not create Job_Preferences table: {ex.Message}");
        }

        // Create Resume table if it doesn't exist
        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS ""Resume"" (
                    ""Id"" text NOT NULL PRIMARY KEY,
                    ""UserId"" text NOT NULL UNIQUE REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                    ""FileUrl"" character varying(500) NOT NULL,
                    ""FileName"" character varying(255),
                    ""UploadedDate"" timestamp with time zone NOT NULL DEFAULT now()
                );";
            cmd.ExecuteNonQuery();
            logger.LogInformation("Resume table ensured.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not create Resume table: {ex.Message}");
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
        
        logger.LogInformation("Database seeding completed successfully");
    }

    var testWorkerEmail = "worker.test@example.com";
    var existingWorker = context.Users.FirstOrDefault(u => u.Email == testWorkerEmail);

    if (existingWorker == null)
    {
        var workerUser = new User
        {
            FirstName = "Test",
            LastName = "Worker",
            Email = testWorkerEmail,
            Phone = "555-0303",
            Location = "Remote",
            Title = "Software Developer",
            Bio = "Test job seeker account for development and testing purposes",
            Skills = "JavaScript, React, Node.js",
            UserType = "Worker",
            DesiredJobTitle = "Frontend Developer",
            DesiredLocation = "Remote",
            DesiredSalaryRange = "$60k-$80k",
            DesiredJobType = "Full-time",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true,
            IsAdmin = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        context.Users.Add(workerUser);
        context.SaveChanges();

        logger.LogInformation("Test worker user seeded successfully");
    }
}
catch (Exception ex)
{
    // Log error but allow app to start
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Database initialization failed. App will continue but may not function correctly.");
}

app.Run();
