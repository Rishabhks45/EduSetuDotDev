using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Application.Features.Authentication;
using EduSetu.Application.Features.Authentication.Request;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EduSetu.Components.Accounts
{
    public partial class Login
    {

        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private IOptions<EncryptionSettings> EncryptionOptions { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
        [Inject] private IPasswordEncryptionService _passwordEncryptionService { get; set; } = default!;

        private LoginDto loginRequest = new();
        private string errorMessage = string.Empty;
        private string successMessage = string.Empty;
        private bool isLoading = false;
        private bool IsLogoutEvent = false;
        private bool showPassword = false;


        protected override async Task OnInitializedAsync()
        {
            await CheckQueryParameters();
            await CheckAuthenticationState();
        }

        private Task CheckQueryParameters()
        {
            Uri uri = new Uri(Navigation.Uri);
            System.Collections.Specialized.NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);

            IsLogoutEvent = queryParams["event"] == "logout";

            return Task.CompletedTask;
        }

        private async Task CheckAuthenticationState()
        {
            AuthenticationState authState = await AuthProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true && !IsLogoutEvent)
            {
                // Check for return URL
                Uri uri = new Uri(Navigation.Uri);
                System.Collections.Specialized.NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string? returnUrl = queryParams["returnUrl"];

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    // User is already logged in and has a return URL, redirect there
                    Navigation.NavigateTo(Uri.UnescapeDataString(returnUrl), replace: true);
                    return;
                }

                // No return URL, redirect based on user role
                string roleString = authState.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
                string dashboardUrl;

                if (string.IsNullOrWhiteSpace(roleString) || !CommonHelper.TryParseEnum<UserRole>(roleString, out UserRole role))
                {
                    dashboardUrl = "/not-authorized";
                }
                else
                {
                    dashboardUrl = CommonHelper.GetDashboardUrlByRole(role);
                }

                Navigation.NavigateTo(dashboardUrl, replace: true);
            }
            else
            {
                await TryLoadSavedEmail();
            }
        }


        private Task TryLoadSavedEmail()
        {
            HttpContext? httpContext = HttpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue("rememberedEmail", out string? savedEmail))
            {
                loginRequest.Email = savedEmail;
            }

            return Task.CompletedTask;
        }

        private async Task HandleLogin()
        {
            // Clear all messages at the start
            errorMessage = string.Empty;
            successMessage = string.Empty;

            // Set loading state immediately when button is clicked
            isLoading = true;
            StateHasChanged();

            // Get master key from encryption settings
            EncryptionSettings encryptionSettings = EncryptionOptions.Value;
            if (!encryptionSettings.IsValid())
            {
                errorMessage = "Configuration error. Please contact support.";
                isLoading = false;
                StateHasChanged();
                return;
            }
            string masterKey = encryptionSettings.MasterKey;

            // Use CQRS pattern for login validation
            LoginDto loginDto = new LoginDto
            {
                Email = loginRequest.Email,
                Password = loginRequest.Password,
                RememberMe = loginRequest.RememberMe,
                MasterKey = masterKey
            };

            LoginResponse loginResponse = await Mediator.Send(new LoginRequest(loginDto));

            if (loginResponse.HasError)
            {
                errorMessage = loginResponse.Errors.Count > 0
                    ? loginResponse.Errors[0].Message
                    : "Invalid email or password";
                isLoading = false;
                StateHasChanged();
                return;
            }

            // Redirect to authentication endpoint with the login token
            // Check for return URL to pass along
            Uri uri = new Uri(Navigation.Uri);
            System.Collections.Specialized.NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            string? returnUrl = queryParams["returnUrl"];

            string signInUrl = $"/api/auth/signin?token={loginResponse.Payload?.LoginToken}";
            if (!string.IsNullOrEmpty(returnUrl))
            {
                signInUrl += $"&returnUrl={returnUrl}";
            }

            Navigation.NavigateTo(signInUrl, forceLoad: true);
            // Note: We don't use finally block here because we want to keep the button disabled
            // during successful navigation, and only re-enable it on errors
        }

        private void HandleValidationFailure()
        {
            // Re-enable the button when validation fails
            isLoading = false;
            StateHasChanged();
        }

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }

        private void HandleGoogleLogin()
        {
            Navigation.NavigateTo("/api/auth/google-login", forceLoad: true);
        }
    }
}