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

    [Display(Name = "Teacher")]
    Teacher = 2,

    [Display(Name = "Student")]
    Student = 3,

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

/// <summary>
/// Register Teacher Step Enum
/// </summary>
 #region Register Teacher Step Enum
public enum QualificationType
{
    [Display(Name = "High School (10th)")]
    HighSchool = 1,
    [Display(Name = "Intermediate (12th)")]
    Intermediate = 2,
    [Display(Name = "Diploma")]
    Diploma = 3,
    [Display(Name = "Undergraduate (Bachelor’s)")]
    Undergraduate = 4,
    [Display(Name = "Postgraduate (Master’s)")]
    Postgraduate = 5,
    [Display(Name = "Doctorate (PhD)")]
    Doctorate = 6,
    [Display(Name = "Other")]
    Other = 7
}
public enum TeachingExperience
{
    [Display(Name = "0-1 years")]
    ZeroToOne = 1,
    [Display(Name = "1-3 years")]
    OneToThree = 2,
    [Display(Name = "3-5 years")]
    ThreeToFive = 3,
    [Display(Name = "5-10 years")]
    FiveToTen = 4,
    [Display(Name = "10+ years")]
    TenPlus = 5
}
public enum PreferredTeachingMode
{
    [Display(Name = "Online")]
    Online = 1,
    [Display(Name = "In-Person")]
    InPerson = 2,
    [Display(Name = "Both")]
    Both = 3
}
public enum Courses
{
    [Display(Name = "Mathematics")]
    Mathematics = 1,
    [Display(Name = "Science")]
    Science = 2,
    [Display(Name = "English")]
    English = 3,
    [Display(Name = "History")]
    History = 4,
    [Display(Name = "Geography")]
    Geography = 5,
    [Display(Name = "Computer Science")]
    ComputerScience = 6,
    [Display(Name = "Art")]
    Art = 7,
    [Display(Name = "Physical Education")]
    PhysicalEducation = 8,
    [Display(Name = "Music")]
    Music = 9,
    [Display(Name = "Other")]
    Other = 10
}

// specialization enum
public enum Specialization
{  
    [Display(Name = "Mathematics")]
    Mathematics = 1,
    [Display(Name = "Science")]
    Science = 2,
    [Display(Name = "English")]
    English = 3,
    [Display(Name = "History")]
    History = 4,
    [Display(Name = "Geography")]
    Geography = 5,
    [Display(Name = "Computer Science /IT")]
    ComputerScience = 6,
    [Display(Name = "Art")]
    Art = 7,
    [Display(Name = "Physical Education")]
    PhysicalEducation = 8,
    [Display(Name = "Music")]
    Music = 9,
    [Display(Name = "Electronics & Communication")]
    ElectronicsAndCommunication = 10,
    [Display(Name = "Electrical Engineering")]
    ElectricalEngineering = 11,
    [Display(Name = "Mechanical Engineering")]
    MechanicalEngineering = 12,
    [Display(Name = "Civil Engineering")]
    CivilEngineering = 13,
    [Display(Name = "Chemical Engineering")]
    ChemicalEngineering = 14,
    [Display(Name = "Biotechnology")]
    Biotechnology = 15,
    [Display(Name = "Architecture")]
    Architecture = 16,
    [Display(Name = "Medical / Health Sciences")]
    MedicalAndHealthSciences = 17,
    [Display(Name = "Pharmacy")]
    Pharmacy = 18,
    [Display(Name = "Management / MBA")]
    ManagementAndMBA = 19,
    [Display(Name = "Commerce")]
    Commerce = 20,
    [Display(Name = "Arts / Humanities")]
    ArtsAndHumanities = 21,
    [Display(Name = "Science (Physics / Chemistry / Math)")]
    SciencePhysicsChemistryMath = 22,
    [Display(Name = "Law")]
    Law = 23,    
    [Display(Name = "Other")]
    Other = 24

}
public enum Certifications
{    
    [Display(Name = "CTET (Central Teacher Eligibility Test)")]
    CTET = 1,
    [Display(Name = "State TET (Teacher Eligibility Test)")]
    StateTET = 2,
    [Display(Name = "B.Ed (Bachelor of Education)")]
    BEd = 3,
    [Display(Name = "M.Ed (Master of Education)")]
    MEd = 4,
    [Display(Name = "Diploma in Elementary Education (D.El.Ed)")]
    DElEd = 5,
    [Display(Name = "NTT (Nursery Teacher Training)")]
    NTT = 6,
    [Display(Name = "PTT (Primary Teacher Training)")]
    PTT = 7,
    [Display(Name = "BTC (Basic Training Certificate)")]
    BTC = 8,
    [Display(Name = "Ph.D. in Education")]
    PhDInEducation = 9,
    [Display(Name = "UGC NET (National Eligibility Test)")]
    UGCNET = 10,
    [Display(Name = "SET (State Eligibility Test)")]
    SET = 11,
    [Display(Name = "Montessori Training")]
    MontessoriTraining = 12,
    [Display(Name = "Special Education Certification")]
    SpecialEducationCertification = 13,
    [Display(Name = "Language Teaching Certification (TESOL/TEFL)")]
    LanguageTeachingCertification = 14,
    [Display(Name = "Other")]
    Other = 15
}

#endregion