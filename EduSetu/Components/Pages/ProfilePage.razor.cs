using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class ProfilePage
    {
        private UserProfile? user;
        private UserStats stats = new();
        private List<ActivityItem> recentActivities = new();
        private List<UploadItem> recentUploads = new();

        protected override void OnInitialized()
        {
            // Simulate loading current user data (replace with real user fetching logic)
            user = new UserProfile
            {
                FirstName = "Jane", // Simulate current user's first name
                LastName = "Smith", // Simulate current user's last name
                Email = "jane.smith@example.com", // Simulate current user's email
                JoinDate = DateTime.Now.AddMonths(-8)
            };

            stats = new UserStats
            {
                NotesCount = 24,
                PYQsCount = 15,
                VideosCount = 8,
                DownloadsCount = 156,
                StudyHours = 127
            };

            recentActivities = new List<ActivityItem>
            {
                new() { Type = "upload", Description = "Uploaded Physics Notes for Class 12", TimeAgo = "2 hours ago" },
                new() { Type = "download", Description = "Downloaded Chemistry PYQs 2023", TimeAgo = "1 day ago" },
                new() { Type = "study", Description = "Completed 2 hours of study session", TimeAgo = "2 days ago" },
                new() { Type = "upload", Description = "Shared Mathematics Video Tutorial", TimeAgo = "3 days ago" }
            };

            recentUploads = new List<UploadItem>
            {
                new() { Title = "Physics Notes - Chapter 1", Type = "note", UploadDate = DateTime.Now.AddDays(-1), Downloads = 45, Rating = 4.8 },
                new() { Title = "Chemistry Lab Video", Type = "video", UploadDate = DateTime.Now.AddDays(-2), Downloads = 32, Rating = 4.6 },
                new() { Title = "Mathematics PYQs 2023", Type = "pyq", UploadDate = DateTime.Now.AddDays(-3), Downloads = 67, Rating = 4.9 },
                new() { Title = "Biology Diagrams", Type = "note", UploadDate = DateTime.Now.AddDays(-4), Downloads = 28, Rating = 4.7 }
            };
        }

        private void EditProfile()
        {
            NavigationManager.NavigateTo("/profile/settings");
        }

        private void UploadContent()
        {
            NavigationManager.NavigateTo("/profile/uploads");
        }

        public class UserProfile
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public DateTime? JoinDate { get; set; }
        }

        public class UserStats
        {
            public int NotesCount { get; set; }
            public int PYQsCount { get; set; }
            public int VideosCount { get; set; }
            public int DownloadsCount { get; set; }
            public int StudyHours { get; set; }
        }

        public class ActivityItem
        {
            public string Type { get; set; } = "";
            public string Description { get; set; } = "";
            public string TimeAgo { get; set; } = "";
        }

        public class UploadItem
        {
            public string Title { get; set; } = "";
            public string Type { get; set; } = "";
            public DateTime UploadDate { get; set; }
            public int Downloads { get; set; }
            public double Rating { get; set; }
        }
    }
}