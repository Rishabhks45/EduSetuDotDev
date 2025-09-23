using EduSetu.Application.Features.Authentication;
using EduSetu.Application.Features.Authentication.Request;
using EduSetu.Application.Features.TeacherRegister;
using EduSetu.Application.Features.TeacherRegister.Requests;
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

        private TeacherRegister TeacherformData = new();
        private CoachingDetailsDto InstituteformData = new();
        private EducationDetailsDtos EducationformData = new();
        private StudentDTOs StudentformData { get; set; } = new();
        private bool showPassword = false;
        private bool showConfirmPassword = false;
        private bool isLoading = false;
        private int currentStep = 1;


        private async Task HandleStudentSubmitAsync()
        {
            isLoading = true;
            var Response = await Mediator.Send(new RegisterUserRequest(StudentformData));

            if (!Response.HasError)
            {
                // Registration successful, redirect to login page
                NavigationManager.NavigateTo("/login?registered=true");
                NotificationService.Success("Registration successful! Please log in.");
                isLoading = false;
            }
            else
            {
                // Handle registration failure (e.g., show error message)
                if (Response.Errors.Count > 0)
                {
                NotificationService.Error($"Registration failed: {Response.Errors[0].Message}");
                    isLoading = false;

                }
                else
                    NotificationService.Error("Registration failed: Unknown error");
            }
            isLoading = false;

        }

        // Get Enum Discription
        private string GetEnumDescription<T>(T value) where T : Enum
        {
            var field = typeof(T).GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute != null ? attribute.Name! : value.ToString();
        }

        private async Task HandleFirstStepTeacherSubmitAsync()
        {
            // You can add validation logic here if needed
            NextStep();
            await Task.CompletedTask;
        }

        private async Task HandleTeacherSubmitAsync()
        {
            isLoading = true;
            //add EducationDetails dto into TeacherformData dto
            TeacherformData.Specialization = EducationformData.Specialization;
            TeacherformData.qualificationType = EducationformData.qualificationType;
            TeacherformData.teachingExperience = EducationformData.teachingExperience;
            TeacherformData.certifications = EducationformData.certifications;
            var Response = await Mediator.Send(new RegisterTeacherRequest(TeacherformData));
            if (!Response.HasError)
            {
                // Registration successful, redirect to login page
                //NavigationManager.NavigateTo("/login?registered=true");
                NotificationService.Success("User Create successful! .");
                InstituteformData.TeacherId = Response.Payload;
                isLoading = false;
                NextStep();
            }
            else
            {
                // Handle registration failure (e.g., show error message)
                if (Response.Errors.Count > 0)
                {
                    NotificationService.Error($"Registration failed: {Response.Errors[0].Message}");
                    InstituteformData.TeacherId = Guid.Empty;
                    isLoading = false;

                }
                else
                {
                    isLoading = false;
                    NotificationService.Error("Registration failed: Unknown error");
                }
                   
            }
            isLoading = false;
        }


        // Coaching Details Steps
        private async Task HandleCoachingDetailsSubmitAsync()
        {
            isLoading = true;
            var Response = await Mediator.Send(new UpsertCoachingDetailsRequest(InstituteformData));

            if (!Response.HasError)
            {
                // Registration successful, redirect to login page
                NavigationManager.NavigateTo("/login?registered=true");
                NotificationService.Success("Registration successful! Please log in.");
                isLoading = false;
            }
            else
            {
                // Handle registration failure (e.g., show error message)
                if (Response.Errors.Count > 0)
                {
                    NotificationService.Error($"Registration failed: {Response.Errors[0].Message}");
                    isLoading = false;
                }
                else
                {
                    NotificationService.Error("Registration failed: Unknown error");
                    isLoading = false;
                }
            }
            isLoading = false;
        }

        private (string Title, string Description)[] steps = {
        ("Personal Info", "Basic personal details"),
        ("Education Details", "Educational background"),
        ("Coaching Details", "Username and password")
    };




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



    }
}