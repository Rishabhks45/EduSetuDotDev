namespace EduSetu.Components.Pages
{
    public partial class AnnouncementsPage
    {
        private List<Announcement> announcements = new();
        private List<Announcement> filteredAnnouncements = new();
        private List<CategoryStats> categories = new();
        private List<Announcement> recentAnnouncements = new();
        private AnnouncementStats stats = new();
        private bool isLoading = false;
        private bool showFilters = false;
        private string selectedCategory = "";
        private string selectedPriority = "";
        private string selectedDateRange = "";
        private int currentPage = 1;
        private int totalPages = 1;
        private int pageSize = 10;

        protected override async Task OnInitializedAsync()
        {
            await LoadAnnouncements();
        }

        private async Task LoadAnnouncements()
        {
            isLoading = true;

            // Simulate API call
            await Task.Delay(1000);

            announcements = new List<Announcement>
        {
            new() {
                Id = 1,
                Title = "New Study Material Uploaded",
                Summary = "We've added comprehensive study materials for Class 12 Physics. Includes detailed notes, practice questions, and video explanations.",
                Content = "Complete study material for Class 12 Physics has been uploaded. This includes chapter-wise notes, important formulas, practice questions with solutions, and video explanations for complex topics.",
                Category = "academic",
                Priority = "high",
                Author = "Admin Team",
                PublishDate = DateTime.Now.AddDays(-1),
                ReadTime = 3,
                IsNew = true,
                ImageUrl = ""
            },
            new() {
                Id = 2,
                Title = "Exam Schedule Updates",
                Summary = "Important updates regarding the upcoming examination schedule. Please check the new dates and timings.",
                Category = "exam",
                Priority = "high",
                Author = "Exam Department",
                PublishDate = DateTime.Now.AddDays(-2),
                ReadTime = 2,
                IsNew = true,
                ImageUrl = ""
            },
            new() {
                Id = 3,
                Title = "Website Maintenance Notice",
                Summary = "Scheduled maintenance on Sunday, 2:00 AM - 4:00 AM. The website will be temporarily unavailable during this period.",
                Category = "maintenance",
                Priority = "medium",
                Author = "Technical Team",
                PublishDate = DateTime.Now.AddDays(-3),
                ReadTime = 1,
                IsNew = false,
                ImageUrl = ""
            },
            new() {
                Id = 4,
                Title = "New Features Released",
                Summary = "Introducing new features: Dark mode, offline reading, and improved search functionality. Try them out!",
                Category = "feature",
                Priority = "medium",
                Author = "Development Team",
                PublishDate = DateTime.Now.AddDays(-4),
                ReadTime = 4,
                IsNew = false,
                ImageUrl = ""
            },
            new() {
                Id = 5,
                Title = "Study Group Event",
                Summary = "Join our virtual study group sessions every Saturday. Connect with fellow students and share knowledge.",
                Category = "event",
                Priority = "low",
                Author = "Community Team",
                PublishDate = DateTime.Now.AddDays(-5),
                ReadTime = 2,
                IsNew = false,
                ImageUrl = ""
            }
        };

            categories = new List<CategoryStats>
        {
            new() { Name = "Academic", Count = 15 },
            new() { Name = "Exam Updates", Count = 8 },
            new() { Name = "Events", Count = 12 },
            new() { Name = "Maintenance", Count = 3 },
            new() { Name = "New Features", Count = 6 }
        };

            recentAnnouncements = announcements.Take(3).ToList();

            stats = new AnnouncementStats
            {
                TotalAnnouncements = announcements.Count,
                UnreadCount = 3,
                ThisWeekCount = 5
            };

            ApplyFilters();
            isLoading = false;
        }

        private void ShowFilters()
        {
            showFilters = !showFilters;
        }

        private void RefreshAnnouncements()
        {
            _ = LoadAnnouncements();
        }

        private void ApplyFilters()
        {
            var filtered = announcements.AsEnumerable();

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                filtered = filtered.Where(a => a.Category == selectedCategory);
            }

            if (!string.IsNullOrEmpty(selectedPriority))
            {
                filtered = filtered.Where(a => a.Priority == selectedPriority);
            }

            if (!string.IsNullOrEmpty(selectedDateRange))
            {
                var now = DateTime.Now;
                filtered = selectedDateRange switch
                {
                    "today" => filtered.Where(a => a.PublishDate.Date == now.Date),
                    "week" => filtered.Where(a => a.PublishDate >= now.AddDays(-7)),
                    "month" => filtered.Where(a => a.PublishDate >= now.AddMonths(-1)),
                    _ => filtered
                };
            }

            filteredAnnouncements = filtered.ToList();
            currentPage = 1;
            totalPages = (int)Math.Ceiling((double)filteredAnnouncements.Count / pageSize);
        }

        private void FilterByCategory(string category)
        {
            selectedCategory = category;
            ApplyFilters();
        }

        private void ReadAnnouncement(Announcement announcement)
        {
            // Navigate to detailed view or show modal
            Console.WriteLine($"Reading announcement: {announcement.Title}");
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
            }
        }

        private void GoToPage(int page)
        {
            currentPage = page;
        }

        private string GetPriorityColor(string priority)
        {
            return priority switch
            {
                "high" => "bg-red-500",
                "medium" => "bg-yellow-500",
                "low" => "bg-green-500",
                _ => "bg-gray-500"
            };
        }

        private string GetCategoryBadgeColor(string category)
        {
            return category switch
            {
                "academic" => "bg-blue-100 text-blue-800",
                "exam" => "bg-red-100 text-red-800",
                "event" => "bg-green-100 text-green-800",
                "maintenance" => "bg-yellow-100 text-yellow-800",
                "feature" => "bg-purple-100 text-purple-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

        public class Announcement
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Summary { get; set; } = "";
            public string Content { get; set; } = "";
            public string Category { get; set; } = "";
            public string Priority { get; set; } = "";
            public string Author { get; set; } = "";
            public DateTime PublishDate { get; set; }
            public int ReadTime { get; set; }
            public bool IsNew { get; set; }
            public string ImageUrl { get; set; } = "";
        }

        public class CategoryStats
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }

        public class AnnouncementStats
        {
            public int TotalAnnouncements { get; set; }
            public int UnreadCount { get; set; }
            public int ThisWeekCount { get; set; }
        }
    }
}