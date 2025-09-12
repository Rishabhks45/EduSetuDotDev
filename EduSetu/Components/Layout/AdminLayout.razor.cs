using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EduSetu.Components.Layout
{
    public partial class AdminLayout
    {
        private bool sidebarOpen = true;
        private bool showNotifications = false;
        private int notifications = 8;
        private string searchQuery = "";
        private User? currentUser;
        private string headerName = "";
        private bool isDarkMode = false;
        private DotNetObjectReference<AdminLayout>? objRef;
        private List<NotificationItem> notificationItems = new()
    {
        new NotificationItem { Id = "1", Type = "error", Title = "Database Connection Timeout", Message = "Multiple connection timeouts to primary database server", Time = "2 hours ago", Severity = "high", Resolved = false },
        new NotificationItem { Id = "2", Type = "warning", Title = "Server Storage Alert", Message = "Storage usage approaching critical threshold", Time = "4 hours ago", Severity = "medium", Resolved = false },
        new NotificationItem { Id = "3", Type = "info", Title = "Maintenance Complete", Message = "System maintenance window completed without issues", Time = "6 hours ago", Severity = "low", Resolved = true },
        new NotificationItem { Id = "4", Type = "warning", Title = "Unusual Login Activity", Message = "Multiple failed login attempts from suspicious IP addresses", Time = "8 hours ago", Severity = "high", Resolved = false }
    };

        // Sidebar items
        private List<AdminSidebarItem> sidebarItems = new()
    {
        new AdminSidebarItem { Id = "overview", Label = "Overview", Icon = "home", Url = "/admin/dashboard", Badge = null, Description = "Dashboard overview" },
        new AdminSidebarItem { Id = "users", Label = "User Management", Icon = "users", Url = "/admin/dashboard/users", Badge = "12", Description = "Manage users and roles" },
        new AdminSidebarItem { Id = "content", Label = "Content Moderation", Icon = "file-text", Url = "/admin/dashboard/content", Badge = "5", Description = "Review and moderate content" },
        new AdminSidebarItem { Id = "analytics", Label = "Analytics", Icon = "bar-chart", Url = "/admin/dashboard/analytics", Badge = null, Description = "Platform analytics" },
        new AdminSidebarItem { Id = "system", Label = "System Health", Icon = "activity", Url = "/admin/dashboard/system", Badge = "2", Description = "Monitor system status" },
        new AdminSidebarItem { Id = "security", Label = "Security", Icon = "lock", Url = "/admin/dashboard/security", Badge = "1", Description = "Security monitoring" },
        new AdminSidebarItem { Id = "reports", Label = "Reports", Icon = "database", Url = "/admin/dashboard/reports", Badge = null, Description = "Generate reports" },
        new AdminSidebarItem { Id = "settings", Label = "Settings", Icon = "settings", Url = "/admin/dashboard/settings", Badge = null, Description = "System configuration" }
    };

        protected override void OnInitialized()
        {
            // Get current user from claims
            var user = HttpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                currentUser = new User
                {
                    FirstName = user.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? "",
                    LastName = user.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "",
                    Email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "",
                    Role = UserRole.SuperAdmin
                };
            }
            // Set headerName to current nav item label or default to "Overview"
            UpdateHeaderName();
            NavigationManager.LocationChanged += OnLocationChanged;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);

                // Initialize theme preference from localStorage
                isDarkMode = await JSRuntime.InvokeAsync<bool>("getThemePreference");
                StateHasChanged();

                // Apply the initial theme
                await ApplyTheme();
            }
        }

        private async Task ToggleTheme()
        {
            isDarkMode = !isDarkMode;
            await ApplyTheme();
            StateHasChanged();
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
        }

        public async ValueTask DisposeAsync()
        {
            if (objRef is not null)
            {
                objRef.Dispose();
            }
        }
        private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            UpdateHeaderName();
            StateHasChanged();
        }

        private void UpdateHeaderName()
        {
            headerName = GetCurrentPageTitle();
            if (string.IsNullOrWhiteSpace(headerName) || headerName == "Dashboard")
            {
                headerName = "Overview";
            }
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        private void ToggleSidebar()
        {
            sidebarOpen = !sidebarOpen;
        }

        private void NavigateToPage(string url)
        {
            NavigationManager.NavigateTo(url);
            // No need to update headerName here, it will be handled by LocationChanged event
        }

        private bool IsActiveTab(string tabId)
        {
            var currentUri = NavigationManager.Uri;
            var baseUri = NavigationManager.BaseUri;
            var relativePath = currentUri.Replace(baseUri, "").TrimStart('/');

            var item = sidebarItems.FirstOrDefault(x => x.Id == tabId);
            if (item != null)
            {
                var itemPath = item.Url.TrimStart('/');

                // Special handling for Overview (home) page
                if (tabId == "overview")
                {
                    // Overview should be active for exact match or when no other specific page is active
                    var isExactMatch = relativePath.Equals(itemPath, StringComparison.OrdinalIgnoreCase);
                    var isBasePath = relativePath.Equals("admin/dashboard", StringComparison.OrdinalIgnoreCase);
                    return isExactMatch || isBasePath;
                }

                var isActive = relativePath.StartsWith(itemPath, StringComparison.OrdinalIgnoreCase);
                return isActive;
            }
            return false;
        }

        private string GetCurrentPageTitle()
        {
            // Get the relative path, remove query strings and trailing slashes
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
                                                .Split('?')[0]
                                                .Trim('/')
                                                .ToLowerInvariant();

            // Normalize item URLs and compare
            var matchedItem = sidebarItems
                .OrderByDescending(x => x.Url.Length) // Ensure longest match comes first
                .FirstOrDefault(x =>
                {
                    var itemPath = x.Url.Trim('/').ToLowerInvariant();
                    return relativePath == itemPath || relativePath.StartsWith(itemPath + "/");
                });

            // Fallback title
            return matchedItem?.Label ?? $"Dashboard";
        }


        private void HandleLogout()
        {
            NavigationManager.NavigateTo("/api/auth/logout", forceLoad: true);
        }

        private void HandleSearch()
        {
            // Handle search logic here
        }

        // Data model for Admin Sidebar
        public class AdminSidebarItem
        {
            public string Id { get; set; } = "";
            public string Label { get; set; } = "";
            public string Icon { get; set; } = "";
            public string Url { get; set; } = "";
            public string? Badge { get; set; }
            public string Description { get; set; } = "";
        }

        public class NotificationItem
        {
            public string Id { get; set; } = "";
            public string Type { get; set; } = "";
            public string Title { get; set; } = "";
            public string Message { get; set; } = "";
            public string Time { get; set; } = "";
            public string Severity { get; set; } = "";
            public bool Resolved { get; set; }
        }
    }
}