using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;

namespace EduSetu.Components.Pages.Dashboard;

public partial class AdminDashboard
{
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool sidebarOpen = true;
    private string activeTab = "overview";
    private int notifications = 8;
    private User? currentUser;

    // Sidebar items
    private List<SidebarItem> sidebarItems = new()
    {
        new SidebarItem { Id = "overview", Label = "Overview", Icon = "home", Badge = null },
        new SidebarItem { Id = "users", Label = "User Management", Icon = "users", Badge = "12" },
        new SidebarItem { Id = "content", Label = "Content Moderation", Icon = "file-text", Badge = "5" },
        new SidebarItem { Id = "analytics", Label = "Analytics", Icon = "bar-chart", Badge = null },
        new SidebarItem { Id = "system", Label = "System Health", Icon = "activity", Badge = "2" },
        new SidebarItem { Id = "security", Label = "Security", Icon = "lock", Badge = "1" },
        new SidebarItem { Id = "reports", Label = "Reports", Icon = "database", Badge = null },
        new SidebarItem { Id = "settings", Label = "Settings", Icon = "settings", Badge = null }
    };

    // Stats data
    private List<StatItem> stats = new()
    {
        new StatItem { Label = "Total Users", Value = "12,847", Change = "+12%", ChangeType = "positive", Icon = "users", Color = "text-blue-600", BgColor = "bg-blue-100" },
        new StatItem { Label = "Content Items", Value = "8,392", Change = "+8%", ChangeType = "positive", Icon = "file-text", Color = "text-green-600", BgColor = "bg-green-100" },
        new StatItem { Label = "Video Hours", Value = "2,847", Change = "+15%", ChangeType = "positive", Icon = "video", Color = "text-purple-600", BgColor = "bg-purple-100" },
        new StatItem { Label = "Monthly Revenue", Value = "$24,891", Change = "+23%", ChangeType = "positive", Icon = "dollar-sign", Color = "text-yellow-600", BgColor = "bg-yellow-100" },
        new StatItem { Label = "Server Uptime", Value = "99.9%", Change = "+0.1%", ChangeType = "positive", Icon = "server", Color = "text-indigo-600", BgColor = "bg-indigo-100" },
        new StatItem { Label = "Support Tickets", Value = "23", Change = "-15%", ChangeType = "negative", Icon = "message-square", Color = "text-red-600", BgColor = "bg-red-100" }
    };

    // Recent users data
    private List<UserItem> recentUsers = new()
    {
        new UserItem { Id = "1", Name = "John Doe", Email = "john@example.com", Role = "user", Status = "active", LastLogin = "2 hours ago", Avatar = "https://images.pexels.com/photos/3769021/pexels-photo-3769021.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=2" },
        new UserItem { Id = "2", Name = "Jane Smith", Email = "jane@example.com", Role = "teacher", Status = "active", LastLogin = "1 day ago", Avatar = "https://images.pexels.com/photos/3769021/pexels-photo-3769021.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=2" },
        new UserItem { Id = "3", Name = "Bob Wilson", Email = "bob@example.com", Role = "user", Status = "pending", LastLogin = "Never", Avatar = "https://images.pexels.com/photos/3769021/pexels-photo-3769021.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=2" },
        new UserItem { Id = "4", Name = "Alice Johnson", Email = "alice@example.com", Role = "teacher", Status = "active", LastLogin = "3 hours ago", Avatar = "https://images.pexels.com/photos/3769021/pexels-photo-3769021.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=2" }
    };

    // Recent content data
    private List<ContentItem> recentContent = new()
    {
        new ContentItem { Id = "1", Title = "Advanced Physics Notes", Author = "Dr. Smith", Type = "notes", Status = "published", UploadDate = "2024-01-20", TypeIcon = "file-text" },
        new ContentItem { Id = "2", Title = "Chemistry Lab Video", Author = "Prof. Johnson", Type = "video", Status = "pending", UploadDate = "2024-01-19", TypeIcon = "video" },
        new ContentItem { Id = "3", Title = "Math PYQ 2024", Author = "Admin", Type = "pyq", Status = "published", UploadDate = "2024-01-18", TypeIcon = "file-text" },
        new ContentItem { Id = "4", Title = "Inappropriate Content Report", Author = "User123", Type = "notes", Status = "flagged", UploadDate = "2024-01-17", TypeIcon = "file-text" }
    };

    protected override void OnInitialized()
    {
        // Get current user from claims
        var user = HttpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            currentUser = new User
            {
                FirstName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? "",
                LastName = user.FindFirst(ClaimTypes.Surname)?.Value ?? "",
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? "",
                Role = UserRole.SuperAdmin // For admin dashboard
            };
        }
    }

    private void ToggleSidebar()
    {
        sidebarOpen = !sidebarOpen;
    }

    private void SetActiveTab(string tabId)
    {
        activeTab = tabId;
    }

    private void HandleLogout()
    {
        NavigationManager.NavigateTo("/logout", true);
    }

    private string GetRoleColor(string role)
    {
        return role switch
        {
            "admin" => "text-red-600 bg-red-100",
            "teacher" => "text-blue-600 bg-blue-100",
            _ => "text-green-600 bg-green-100"
        };
    }

    private string GetStatusColor(string status)
    {
        return status switch
        {
            "active" => "text-green-600 bg-green-100",
            "pending" => "text-yellow-600 bg-yellow-100",
            "flagged" => "text-red-600 bg-red-100",
            _ => "text-gray-600 bg-gray-100"
        };
    }

    private string GetContentTypeColor(string type)
    {
        return type switch
        {
            "notes" => "bg-blue-100 text-blue-600",
            "video" => "bg-red-100 text-red-600",
            "pyq" => "bg-green-100 text-green-600",
            _ => "bg-gray-100 text-gray-600"
        };
    }
}

// Data models
public class SidebarItem
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public string Icon { get; set; } = "";
    public string? Badge { get; set; }
}

public class StatItem
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
    public string Change { get; set; } = "";
    public string ChangeType { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Color { get; set; } = "";
    public string BgColor { get; set; } = "";
}

public class UserItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Status { get; set; } = "";
    public string LastLogin { get; set; } = "";
    public string Avatar { get; set; } = "";
}

public class ContentItem
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public string UploadDate { get; set; } = "";
    public string TypeIcon { get; set; } = "";
} 