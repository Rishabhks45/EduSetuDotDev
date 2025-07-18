using System.ComponentModel.DataAnnotations;

namespace EduSetu.Domain.Enums;

public enum RowStatus : byte
{
    [Display(Name = "Inactive")]
    Inactive = 0,
    
    [Display(Name = "Active")]
    Active = 1,
    
    [Display(Name = "Deleted")]
    Deleted = 255
}


/// <summary>
/// Defines the available user roles in the application
/// </summary>
public enum UserRole
{
    [Display(Name = "None")]
    None = 0,

    [Display(Name = "Super Admin")]
    SuperAdmin = 1,

    [Display(Name = "Teacher Manager")]
    PortfolioManager = 2,

    [Display(Name = "Student")]
    HotelStaff = 3,

}

/// <summary>
/// Defines the subscription profile status
/// </summary>
public enum ProfileStatus
{
    [Display(Name = "Canceled")]
    None = 0,

    [Display(Name = "Public")]
    Active = 1,

    [Display(Name = "Pending")]
    Trialing = 2,   

}
