using EduSetu.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Domain.Entities
{
    /// <summary>
    /// User domain entity representing a system user
    /// Clean entity with only properties - business logic moved to command handlers
    /// </summary>
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public QualificationType? qualificationType { get; set; }
        public Specialization? Specialization { get; set; }
        public TeachingExperience? teachingExperience { get; set; }
        public Certifications? certifications { get; set; }
        //public PreferredTeachingMode? PreferredTeachingMode { get; set; }


        // Full name computed property
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Parameterless constructor for EF Core
        public User() { }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public User(string firstName, string lastName, string email, string password, UserRole role, string? instituteName = null)
        {
            FirstName = firstName?.Trim() ?? string.Empty;
            LastName = lastName?.Trim() ?? string.Empty;
            Email = email?.Trim() ?? string.Empty;
            Password = password?.Trim() ?? string.Empty;
            Role = role;
        }
    }

}
