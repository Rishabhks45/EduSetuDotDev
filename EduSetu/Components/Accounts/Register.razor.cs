using EduSetu.Application.Features.Authentication;
using EduSetu.Application.Features.Authentication.Request;
using EduSetu.Domain.Enums;
using EduSetu.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EduSetu.Components.Accounts
{
    public partial class Register
    {
        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private INotificationService NotificationService { get; set; } = default!;


        private UserRole selectedUserRole = UserRole.Student;

        private RegisterFormData formData = new();
        private Institute InstituteformData = new();
        private StudentDTOs StudentformData { get; set; } = new();
        private bool showPassword = false;
        private bool showConfirmPassword = false;
        private bool isLoading = false;
        private int currentStep = 1;


        private async Task HandleStudentSubmitAsync()
        {
            var Response = await Mediator.Send(new RegisterUserRequest(StudentformData));

            if (!Response.HasError)
            {
                // Registration successful, redirect to login page
                NavigationManager.NavigateTo("/login?registered=true");
                NotificationService.Success("Registration successful! Please log in.");
            }
            else
            {
                // Handle registration failure (e.g., show error message)
                if (Response.Errors.Count > 0)
                    NotificationService.Error($"Registration failed: {Response.Errors[0].Message}");
                else
                    NotificationService.Error("Registration failed: Unknown error");
            }
        }

     // Get Enum Discription
        private string GetEnumDescription<T>(T value) where T : Enum
        {
            var field = typeof(T).GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute != null ? attribute.Name! : value.ToString();
        }













































        private (string Title, string Description)[] steps = {
        ("Personal Info", "Basic personal details"),
        ("Education Details", "Educational background"),
        ("Coaching Details", "Username and password")
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

        public class Institute
        {
            public int Id { get; set; }

            public string InstituteName { get; set; } = null!;
            public string ModeOfCoaching { get; set; } = null!;
            public int NumberOfStudents { get; set; }
            public string Address { get; set; } = null!;
            public string City { get; set; } = null!;
            public string State { get; set; } = null!;
            public string PinCode { get; set; } = null!;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
        }


        private string GetStepClass(int stepIndex)
        {
            var baseClasses = "flex items-center justify-center rounded-full";
            
            if (stepIndex < currentStep)
            {
                // Completed step
                return $"{baseClasses} bg-primary-600 dark:bg-primary-700 text-white";
            }
            else if (stepIndex == currentStep)
            {
                // Current step
                return $"{baseClasses} bg-primary-100 dark:bg-primary-900 text-primary-700 dark:text-primary-300 border border-primary-600 dark:border-primary-500";
            }
            else
            {
                // Upcoming step
                return $"{baseClasses} bg-gray-100 dark:bg-gray-800 text-gray-500 dark:text-gray-400 border border-gray-300 dark:border-gray-600";
            }
        }

        private string GetStepLineClass(int stepIndex)
        {
            var baseClass = "h-[2px]";
            
            if (stepIndex < currentStep)
            {
                // Completed line
                return $"{baseClass} bg-primary-600 dark:bg-primary-700";
            }
            else
            {
                // Upcoming line
                return $"{baseClass} bg-gray-300 dark:bg-gray-600";
            }
        }
           

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            showConfirmPassword = !showConfirmPassword;
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