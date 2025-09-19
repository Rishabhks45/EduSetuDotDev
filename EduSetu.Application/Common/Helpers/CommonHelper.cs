using EduSetu.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EduSetu.Application.Common.Helpers;

/// <summary>
/// Common utility functions used across the application
/// </summary>
public static class CommonHelper
{


    /// <summary>
    /// Gets the appropriate dashboard URL based on user role enum
    /// </summary>
    /// <param name="role">User role enum</param>
    /// <returns>Dashboard URL for the role</returns>
    public static string GetDashboardUrlByRole(UserRole? role)
    {
        return role switch
        {
            UserRole.SuperAdmin => "/admin/dashboard",
            UserRole.Teacher => "/teacher/dashboard",
            UserRole.Student => "/",
            _ => "/not-authorized" // Unauthorized users
        };
    }

    /// <summary>
    /// Converts a string to an enum value of type T.
    /// Supports matching against enum name or [Display(Name = "...")]
    /// </summary>
    public static bool TryParseEnum<T>(string value, out T result) where T : struct, Enum
    {
        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Match against name
            if (string.Equals(field.Name, value, StringComparison.OrdinalIgnoreCase))
            {
                result = (T)field.GetValue(null)!;
                return true;
            }

            // Match against [Display(Name = "Something")]
            DisplayAttribute? displayAttr = field.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null && string.Equals(displayAttr.Name, value, StringComparison.OrdinalIgnoreCase))
            {
                result = (T)field.GetValue(null)!;
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Converts an enum value to its string representation (the actual enum name).
    /// Returns the enum name as string (e.g., "SuperAdmin").
    /// </summary>
    public static string ToDisplayEnumString<T>(this T enumValue) where T : Enum
    {
        return enumValue.ToString();
    }

    /// <summary>
    /// Converts an enum value to its display description.
    /// Returns the [Display(Name = "...")] value if available, then [Display(Description = "...")] if available, 
    /// otherwise returns the enum name.
    /// </summary>
    public static string ToDisplayEnumDescription<T>(this T enumValue) where T : Enum
    {
        FieldInfo? field = typeof(T).GetField(enumValue.ToString());
        DisplayAttribute? displayAttr = field?.GetCustomAttribute<DisplayAttribute>();

        // Return Display Name first, then Description, then enum name as fallback
        return displayAttr?.Name ?? displayAttr?.Description ?? enumValue.ToString();
    }

    public static string GenerateTemporaryPassword()
    {
        // Define character sets
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string numberChars = "0123456789";
        const string specialChars = "@$!%*?&";

        // Create a cryptographically secure random number generator
        using System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();

        // Ensure at least one character from each required set
        List<char> password = new List<char>
        {
            GetRandomChar(uppercaseChars, rng), // One uppercase
            GetRandomChar(lowercaseChars, rng), // One lowercase
            GetRandomChar(numberChars, rng),     // One number
            GetRandomChar(specialChars, rng)     // One special char
        };

        // Add 4 more random characters from all allowed chars
        string allChars = uppercaseChars + lowercaseChars + numberChars + specialChars;
        for (int i = 0; i < 4; i++)
        {
            password.Add(GetRandomChar(allChars, rng));
        }

        // Shuffle the password characters
        int n = password.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do
            {
                rng.GetBytes(box);
            }
            while (!(box[0] < n * (byte.MaxValue / n)));
            int k = box[0] % n;
            n--;
            char value = password[k];
            password[k] = password[n];
            password[n] = value;
        }

        return new string(password.ToArray());
    }

    private static char GetRandomChar(string chars, System.Security.Cryptography.RNGCryptoServiceProvider rng)
    {
        byte[] randomBytes = new byte[1];
        do
        {
            rng.GetBytes(randomBytes);
        }
        while (!(randomBytes[0] < chars.Length * (byte.MaxValue / chars.Length)));
        return chars[randomBytes[0] % chars.Length];
    }

    /// <summary>
    /// Generates user initials from first and last names
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name (optional)</param>
    /// <returns>User initials (e.g., "JD" for John Doe) or "U" if no valid names provided</returns>
    public static string GetUserInitials(string? firstName, string? lastName = null)
    {
        if (!string.IsNullOrEmpty(firstName))
        {
            string firstInitial = firstName[0].ToString().ToUpper();
            string lastInitial = !string.IsNullOrEmpty(lastName) ? lastName[0].ToString().ToUpper() : "";
            return firstInitial + lastInitial;
        }
        return "U";
    }

}
