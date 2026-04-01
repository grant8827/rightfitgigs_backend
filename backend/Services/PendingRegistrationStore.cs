using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace RightFitGigs.Services
{
    /// <summary>
    /// Holds registration data in memory until the user verifies their email via OTP.
    /// Data is never written to the database until verification succeeds.
    /// </summary>
    public class PendingRegistration
    {
        public required string Email { get; init; }
        public required string HashedPassword { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string Phone { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public string Bio { get; init; } = string.Empty;
        public string Skills { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public required string UserType { get; init; }   // "Worker" | "Employer"

        // Employer-only fields
        public string? CompanyName { get; init; }
        public string? Description { get; init; }
        public string? Industry { get; init; }
        public string? CompanySize { get; init; }
        public string? Website { get; init; }

        public required string OtpCode { get; init; }
        public DateTime ExpiresAt { get; init; }
        public int Attempts { get; set; } = 0;
    }

    public class PendingRegistrationStore
    {
        private readonly ConcurrentDictionary<string, PendingRegistration> _store = new(StringComparer.OrdinalIgnoreCase);
        private const int OtpExpiryMinutes = 10;
        private const int MaxAttempts = 5;

        /// <summary>Saves (or overwrites) a pending registration for the given email.</summary>
        public PendingRegistration Create(PendingRegistration data)
        {
            _store[data.Email] = data;
            return data;
        }

        /// <summary>Generates a new random 6-digit OTP string.</summary>
        public static string GenerateOtp()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            var value = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
            return value.ToString("D6");
        }

        public static DateTime NewExpiry() => DateTime.UtcNow.AddMinutes(OtpExpiryMinutes);

        /// <summary>
        /// Validates the supplied OTP.
        /// Returns null on success (caller should then remove the entry and save user to DB).
        /// Returns an error string on failure.
        /// </summary>
        public (PendingRegistration? Pending, string? Error) Verify(string email, string otp)
        {
            if (!_store.TryGetValue(email, out var pending))
                return (null, "No pending registration found. Please start registration again.");

            if (DateTime.UtcNow > pending.ExpiresAt)
            {
                _store.TryRemove(email, out _);
                return (null, "The verification code has expired. Please register again.");
            }

            pending.Attempts++;

            if (pending.Attempts > MaxAttempts)
            {
                _store.TryRemove(email, out _);
                return (null, "Too many incorrect attempts. Please register again.");
            }

            if (pending.OtpCode != otp.Trim())
                return (null, $"Incorrect code. {MaxAttempts - pending.Attempts + 1} attempt(s) remaining.");

            // Success — remove from store so it can't be reused
            _store.TryRemove(email, out _);
            return (pending, null);
        }

        public bool HasPending(string email) => _store.ContainsKey(email);

        public void Remove(string email) => _store.TryRemove(email, out _);

        /// <summary>Generates a fresh OTP for an existing pending record and resets expiry + attempts.</summary>
        public PendingRegistration? Refresh(string email)
        {
            if (!_store.TryGetValue(email, out var existing))
                return null;

            var refreshed = new PendingRegistration
            {
                Email          = existing.Email,
                HashedPassword = existing.HashedPassword,
                FirstName      = existing.FirstName,
                LastName       = existing.LastName,
                Phone          = existing.Phone,
                Location       = existing.Location,
                Bio            = existing.Bio,
                Skills         = existing.Skills,
                Title          = existing.Title,
                UserType       = existing.UserType,
                CompanyName    = existing.CompanyName,
                Description    = existing.Description,
                Industry       = existing.Industry,
                CompanySize    = existing.CompanySize,
                Website        = existing.Website,
                OtpCode        = GenerateOtp(),
                ExpiresAt      = NewExpiry()
            };

            _store[email] = refreshed;
            return refreshed;
        }
    }
}
