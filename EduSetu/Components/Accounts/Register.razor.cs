using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace EduSetu.Components.Accounts
{
    public partial class Register
    {
        private RegisterFormData formData = new();
        private bool showPassword = false;
        private bool showConfirmPassword = false;
        private bool isLoading = false;
        private int currentStep = 1;
        private string interestInput = "";

        private (string Title, string Description)[] steps = {
        ("Personal Info", "Basic personal details"),
        ("Academic Info", "Educational background"),
        ("Account Setup", "Username and password"),
        ("Preferences", "Interests and bio")
    };

        private (string Value, string Label)[] institutionTypes = {
        ("school", "School"),
        ("university", "University/College"),
        ("other", "Other")
    };

        private string[] boards = { "CBSE", "ICSE", "State Board", "IB", "NIOS" };
        private string[] institutions = {
        "Delhi University", "Mumbai University", "Bangalore University", "IIT Bombay", "IIT Delhi",
        "CBSE Schools", "ICSE Schools", "State Board Schools", "Private Institutions"
    };
        private string[] courses = {
        "B.Tech Computer Science", "B.Tech Engineering", "MBBS", "B.Sc", "B.Com", "B.A",
        "M.Tech", "MBA", "Class 10", "Class 11", "Class 12", "JEE Preparation", "NEET Preparation"
    };
        private string[] semesters = { "Semester 1", "Semester 2", "Semester 3", "Semester 4", "Semester 5", "Semester 6", "Semester 7", "Semester 8" };
        private string[] years = { "2024", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015" };
        private string[] subjectInterests = {
        "Physics", "Chemistry", "Mathematics", "Biology", "Computer Science", "English",
        "History", "Geography", "Economics", "Political Science", "Psychology", "Philosophy",
        "Engineering", "Medicine", "Business", "Arts", "Literature", "Music", "Sports"
    };

        public class RegisterFormData
        {
            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; } = "";

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; } = "";

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = "";

            public string PhoneNumber { get; set; } = "";
            public DateTime? DateOfBirth { get; set; }

            [Required(ErrorMessage = "Institution is required")]
            public string Institution { get; set; } = "";

            public string InstitutionType { get; set; } = "university";

            [Required(ErrorMessage = "Course is required")]
            public string Course { get; set; } = "";

            public string Board { get; set; } = "";
            public string Semester { get; set; } = "";
            public string Year { get; set; } = DateTime.Now.Year.ToString();

            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
            public string Username { get; set; } = "";

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Please confirm your password")]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; } = "";

            public List<string> Interests { get; set; } = new();
            public string Bio { get; set; } = "";

            [Required(ErrorMessage = "You must agree to the terms and conditions")]
            public bool AgreeToTerms { get; set; } = false;

            public bool SubscribeNewsletter { get; set; } = true;
        }

        private string GetStepClass(int stepIndex)
        {
            if (currentStep > stepIndex + 1)
                return "flex items-center justify-center w-10 h-10 rounded-full border-2 bg-primary-600 border-primary-600 text-white";
            else if (currentStep >= stepIndex + 1)
                return "flex items-center justify-center w-10 h-10 rounded-full border-2 bg-primary-600 border-primary-600 text-white";
            else
                return "flex items-center justify-center w-10 h-10 rounded-full border-2 border-gray-300 text-gray-400";
        }

        private string GetStepLineClass(int stepIndex)
        {
            if (currentStep > stepIndex + 1)
                return "w-16 h-0.5 mx-4 bg-primary-600";
            else
                return "w-16 h-0.5 mx-4 bg-gray-300";
        }

        private string GetInstitutionTypeClass(string value)
        {
            return formData.InstitutionType == value
                ? "border-primary-500 bg-primary-50 text-primary-700"
                : "border-gray-200 hover:border-gray-300";
        }

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            showConfirmPassword = !showConfirmPassword;
        }

        private void SelectInstitutionType(string type)
        {
            formData.InstitutionType = type;
            StateHasChanged();
        }

        private void AddInterest()
        {
            if (!string.IsNullOrWhiteSpace(interestInput) && !formData.Interests.Contains(interestInput.Trim()))
            {
                formData.Interests.Add(interestInput.Trim());
                interestInput = "";
                StateHasChanged();
            }
        }

        private void RemoveInterest(string interest)
        {
            formData.Interests.Remove(interest);
            StateHasChanged();
        }

        private void AddSubjectInterest(string subject)
        {
            if (!formData.Interests.Contains(subject))
            {
                formData.Interests.Add(subject);
                StateHasChanged();
            }
        }

        private void NextStep()
        {
            if (currentStep < steps.Length)
            {
                currentStep++;
                StateHasChanged();
            }
        }

        private void PreviousStep()
        {
            if (currentStep > 1)
            {
                currentStep--;
                StateHasChanged();
            }
        }

        private async Task HandleGoogleSignup()
        {
            // Simulate Google signup
            Console.WriteLine("Google signup clicked");
        }

        private async Task HandleSubmit()
        {
            isLoading = true;

            // Simulate API call
            await Task.Delay(2000);

            Console.WriteLine($"Registration: {formData.FirstName} {formData.LastName}, Email: {formData.Email}");

            // Redirect to login page
            NavigationManager.NavigateTo("/login?registered=true");

            isLoading = false;
        }
    }
}