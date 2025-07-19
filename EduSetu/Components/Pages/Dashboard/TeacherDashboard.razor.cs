using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;

namespace EduSetu.Components.Pages.Dashboard;

public partial class TeacherDashboard
{
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool sidebarOpen = true;
    private string activeTab = "overview";
    private int notifications = 5;
    private User? currentUser;

    // Sidebar items
    private List<TeacherSidebarItem> sidebarItems = new()
    {
        new TeacherSidebarItem { Id = "overview", Label = "Overview", Icon = "home", Badge = null },
        new TeacherSidebarItem { Id = "my-content", Label = "My Content", Icon = "file-text", Badge = "12" },
        new TeacherSidebarItem { Id = "resources", Label = "Resources", Icon = "book-open", Badge = null },
        new TeacherSidebarItem { Id = "schedule", Label = "Schedule", Icon = "calendar", Badge = "3" },
        new TeacherSidebarItem { Id = "messages", Label = "Messages", Icon = "message-square", Badge = "8" },
        new TeacherSidebarItem { Id = "analytics", Label = "Performance Analytics", Icon = "bar-chart", Badge = null },
        new TeacherSidebarItem { Id = "settings", Label = "Settings", Icon = "settings", Badge = null }
    };

    // Stats data
    private List<TeacherStatItem> stats = new()
    {
        new TeacherStatItem { Label = "Total Students", Value = "247", Change = "+12%", ChangeType = "positive", Icon = "users", Color = "text-blue-600", BgColor = "bg-blue-100" },
        new TeacherStatItem { Label = "Content Views", Value = "15,420", Change = "+18%", ChangeType = "positive", Icon = "eye", Color = "text-green-600", BgColor = "bg-green-100" },
        new TeacherStatItem { Label = "Downloads", Value = "2,845", Change = "+25%", ChangeType = "positive", Icon = "download", Color = "text-purple-600", BgColor = "bg-purple-100" },
        new TeacherStatItem { Label = "Avg Rating", Value = "4.8", Change = "+0.2", ChangeType = "positive", Icon = "trending-up", Color = "text-yellow-600", BgColor = "bg-yellow-100" }
    };

    // Upcoming classes data
    private List<ClassItem> upcomingClasses = new()
    {
        new ClassItem { Id = "1", Subject = "Data Structures", Time = "10:00 AM", Students = 45, Room = "CS-101" },
        new ClassItem { Id = "2", Subject = "Algorithms", Time = "2:00 PM", Students = 38, Room = "CS-102" },
        new ClassItem { Id = "3", Subject = "Database Systems", Time = "4:00 PM", Students = 52, Room = "CS-103" }
    };

    // Recent activity data
    private List<ActivityItem> recentActivity = new()
    {
        new ActivityItem { Id = "1", Type = "upload", Message = "New video lecture uploaded: \"Advanced Algorithms\"", Time = "2 hours ago", TypeIcon = "upload" },
        new ActivityItem { Id = "2", Type = "comment", Message = "Student commented on \"Data Structures Notes\"", Time = "4 hours ago", TypeIcon = "message-circle" },
        new ActivityItem { Id = "3", Type = "download", Message = "25 new downloads on \"Database Systems Guide\"", Time = "6 hours ago", TypeIcon = "download" },
        new ActivityItem { Id = "4", Type = "rating", Message = "Received 5-star rating on \"Machine Learning Basics\"", Time = "1 day ago", TypeIcon = "star" }
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
                Role = UserRole.Teacher // For teacher dashboard
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

    private string GetActivityTypeColor(string type)
    {
        return type switch
        {
            "upload" => "bg-blue-100 text-blue-600",
            "comment" => "bg-green-100 text-green-600",
            "download" => "bg-purple-100 text-purple-600",
            "rating" => "bg-yellow-100 text-yellow-600",
            _ => "bg-gray-100 text-gray-600"
        };
    }
}

// Data models for Teacher Dashboard
public class TeacherSidebarItem
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public string Icon { get; set; } = "";
    public string? Badge { get; set; }
}

public class TeacherStatItem
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
    public string Change { get; set; } = "";
    public string ChangeType { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Color { get; set; } = "";
    public string BgColor { get; set; } = "";
}

public class ClassItem
{
    public string Id { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Time { get; set; } = "";
    public int Students { get; set; }
    public string Room { get; set; } = "";
}

public class ActivityItem
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";
    public string Message { get; set; } = "";
    public string Time { get; set; } = "";
    public string TypeIcon { get; set; } = "";
} 