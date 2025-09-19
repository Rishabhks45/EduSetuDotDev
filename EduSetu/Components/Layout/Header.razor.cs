using EduSetu.Services.Implementations;
using EduSetu.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace EduSetu.Components.Layout
{
    public partial class Header
    {
        [Inject] private IFileUploadService FileUploadService { get; set; } = default!;

        private bool isScrolled = false;
        private bool isMenuOpen = false;
        private ClaimsPrincipal? user;
        private string? userName;
        private string? UserProfileUrl { get; set; }
        private string? userEmail;
        private bool isAuthenticated;
        private bool showUserDropdown = false;
        private DotNetObjectReference<Header>? objRef;
        private bool isDarkMode = false; // Default to light mode
        private ElementReference dropdownContainer;

        protected override async Task OnInitializedAsync()
        {
            GetHeaderStyle();
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            user = authState.User;
            isAuthenticated = user.Identity?.IsAuthenticated == true;
            if (isAuthenticated)
            {
                userName = user.Identity?.Name ?? user.FindFirst("name")?.Value ?? user.FindFirst("given_name")?.Value;
                userEmail = user.FindFirst(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value;
                UserProfileUrl = user.FindFirst("ProfilePictureUrl")?.Value;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("addScrollListener", objRef);

                // Initialize theme preference from localStorage
                isDarkMode = await JSRuntime.InvokeAsync<bool>("getThemePreference");
                
                // Initialize dropdown click-outside handling
                await JSRuntime.InvokeVoidAsync("initializeDropdown", dropdownContainer, objRef);
                await JSRuntime.InvokeVoidAsync("setHeaderReference", objRef);


                StateHasChanged();
                // Apply the initial theme
                await ApplyTheme();
            }
        }

        [JSInvokable]
        public void OnScroll(bool scrolled)
        {
            isScrolled = scrolled;
            StateHasChanged();
        }

        private string GetHeaderStyle()
        {
            var backgroundColor = isScrolled
                ? (isDarkMode ? "#1f2937" : "white") // dark:bg-gray-800 when scrolled 
                : "transparent";
            var padding = isScrolled ? "0.5rem 0" : "1rem 0";
            var boxShadow = isScrolled
                ? (isDarkMode ? "0 4px 6px -1px rgba(0, 0, 0, 0.2), 0 2px 4px -1px rgba(0, 0, 0, 0.3)" : "0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)")
                : "none";

            return $"background-color: {backgroundColor}; padding: {padding}; box-shadow: {boxShadow};";
        }

        private void ToggleMenu()
        {
            isMenuOpen = !isMenuOpen;
        }

        private void ToggleUserDropdown()
        {
            showUserDropdown = !showUserDropdown;
        }

        private async Task ToggleTheme()
        {
            isDarkMode = !isDarkMode;
            await ApplyTheme();
            StateHasChanged(); // Trigger re-render to update header styles
        }

        private async Task ApplyTheme()
        {
            // Save preference to localStorage and apply theme to document
            await JSRuntime.InvokeVoidAsync("setThemePreference", isDarkMode);

            // Apply the theme classes to the HTML element
            if (isDarkMode)
            {
                await JSRuntime.InvokeVoidAsync("eval", "document.documentElement.classList.add('dark')");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("eval", "document.documentElement.classList.remove('dark')");
            }
            
            // Trigger re-render to update header styles based on new theme
            StateHasChanged();
        }

        private void Logout()
        {
            Navigation.NavigateTo("/api/auth/logout", forceLoad: true);
        }

        [JSInvokable]
        public void CloseDropdown()
        {
            showUserDropdown = false;
            StateHasChanged();
        }

        [JSInvokable]
        public void SetUserInfo(string name, string? email, string? profileUrl)
        {
            userName = !string.IsNullOrEmpty(name) ? name : "Guest User";
            userEmail = !string.IsNullOrEmpty(email) ? email : "guest@example.com";
            UserProfileUrl = !string.IsNullOrEmpty(profileUrl) ? profileUrl : null;
            showUserDropdown = false; // Close dropdown after update
            StateHasChanged();
        }

        private bool IsProfileImageExists()
        {
            if (string.IsNullOrEmpty(UserProfileUrl))
            {
                return false;
            }

            return FileUploadService.FileExists(UserProfileUrl);
        }

        public async ValueTask DisposeAsync()
        {
            if (objRef is not null)
            {
                objRef.Dispose();
            }
        }
    }
}