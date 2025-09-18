using EduSetu.Domain.Enums;

namespace EduSetu.Domain.Entities;

public class CoachingDetails : BaseEntity
{
    public string InstituteName { get; set; } = string.Empty;

    public Guid TeacherId { get; set; }

    public PreferredTeachingMode PreferredTeachingMode { get; set; }

    public int NumberOfStudents { get; set; }

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty; 

    public string State { get; set; } = string.Empty;

    public string PinCode { get; set; } = string.Empty;
}
