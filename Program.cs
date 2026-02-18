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
    "http://localhost:5173",
    "http://127.0.0.1:5173"
}
.Where(origin => !string.IsNullOrWhiteSpace(origin))
.Select(origin => origin!.Trim().TrimEnd('/'))
.Distinct(StringComparer.OrdinalIgnoreCase)
.ToArray();

// Add Entity Framework
var databaseUrl = builder.Configuration["DATABASE_URL"];
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var hasPostgresConnection = !string.IsNullOrWhiteSpace(databaseUrl) ||
                            (!string.IsNullOrWhiteSpace(defaultConnection) &&
                             (defaultConnection.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                              defaultConnection.Contains("Username=", StringComparison.OrdinalIgnoreCase)));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (hasPostgresConnection)
    {
        var postgresConnection = !string.IsNullOrWhiteSpace(databaseUrl)
            ? ConvertDatabaseUrlToNpgsqlConnectionString(databaseUrl)
            : defaultConnection!;

        options.UseNpgsql(postgresConnection);
    }
    else
    {
        options.UseSqlite(defaultConnection ?? "Data Source=rightfitgigs.db");
    }

    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(allowedOrigins)
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
    if (hasPostgresConnection)
    {
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
    }
    else
    {
        context.Database.Migrate();
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

static string ConvertDatabaseUrlToNpgsqlConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);

    if (userInfo.Length != 2)
    {
        throw new InvalidOperationException("DATABASE_URL is missing username or password.");
    }

    var builder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.Trim('/'),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = Uri.UnescapeDataString(userInfo[1]),
        SslMode = SslMode.Require
    };

    return builder.ConnectionString;
}
