using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace EduSetu.Components.Accounts
{
    public partial class Login
    {
        private LoginFormData formData = new();
        private bool showPassword = false;
        private bool isLoading = false;

        public class LoginFormData
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = "";

            public bool RememberMe { get; set; } = false;
        }

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private async Task HandleSubmit()
        {
            isLoading = true;

            // Simulate API call
            await Task.Delay(1000);

            Console.WriteLine($"Login attempt: Email={formData.Email}, RememberMe={formData.RememberMe}");

            // In a real app, you would call your authentication service here
            // For now, just redirect to home page
            NavigationManager.NavigateTo("/");

            isLoading = false;
        }

        private void HandleGoogleLogin()
        {
            // In a real app, this would integrate with Google OAuth
            Console.WriteLine("Google login clicked");
            // You would implement Google OAuth integration here
        }
    }
}