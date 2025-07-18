namespace EduSetu.Components.Pages
{
    public partial class HistoryPage
    {
        private List<ActivityItem> activities = new();
        private List<ActivityItem> filteredActivities = new();
        private List<ActivityItem> paginatedActivities = new();
        private ActivitySummary summary = new();
        private List<WeeklyActivity> weeklyActivity = new();
        private List<SubjectStats> topSubjects = new();
        private string selectedActivityType = "";
        private string selectedDateRange = "";
        private string sortBy = "date";
        private int currentPage = 1;
        private int totalPages = 1;
        private int pageSize = 20;

        private string SelectedActivityType
        {
            get => selectedActivityType;
            set
            {
                selectedActivityType = value;
                ApplyFilters();
            }
        }

        private string SelectedDateRange
        {
            get => selectedDateRange;
            set
            {
                selectedDateRange = value;
                ApplyFilters();
            }
        }

        private string SortBy
        {
            get => sortBy;
            set
            {
                sortBy = value;
                ApplyFilters();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            activities = new List<ActivityItem>
        {
            new() {
                Id = 1,
                Type = "download",
                Title = "Downloaded Physics Notes",
                Description = "Class 12 Physics Chapter 1 - Electric Charges and Fields",
                Timestamp = DateTime.Now.AddHours(-2),
                Duration = TimeSpan.FromMinutes(5),
                FileSize = "2.4 MB"
            },
            new() {
                Id = 2,
                Type = "study",
                Title = "Study Session",
                Description = "Mathematics - Integration and Differentiation",
                Timestamp = DateTime.Now.AddHours(-4),
                Duration = TimeSpan.FromMinutes(45)
            },
            new() {
                Id = 3,
                Type = "upload",
                Title = "Uploaded Chemistry Lab Report",
                Description = "Experiment 5 - Acid-Base Titration",
                Timestamp = DateTime.Now.AddDays(-1),
                Duration = TimeSpan.FromMinutes(15),
                FileSize = "1.8 MB"
            },
            new() {
                Id = 4,
                Type = "search",
                Title = "Searched for Biology Notes",
                Description = "Cell Division and Mitosis",
                Timestamp = DateTime.Now.AddDays(-1).AddHours(-2),
                Duration = TimeSpan.FromMinutes(3)
            },
            new() {
                Id = 5,
                Type = "download",
                Title = "Downloaded PYQ Papers",
                Description = "Mathematics Previous Year Questions 2022",
                Timestamp = DateTime.Now.AddDays(-2),
                Duration = TimeSpan.FromMinutes(8),
                FileSize = "3.1 MB"
            },
            new() {
                Id = 6,
                Type = "study",
                Title = "Study Session",
                Description = "Physics - Wave Optics and Interference",
                Timestamp = DateTime.Now.AddDays(-2).AddHours(-3),
                Duration = TimeSpan.FromMinutes(60)
            },
            new() {
                Id = 7,
                Type = "login",
                Title = "Logged into EduSetu",
                Description = "Session started",
                Timestamp = DateTime.Now.AddDays(-3),
                Duration = TimeSpan.FromMinutes(1)
            }
        };

            summary = new ActivitySummary
            {
                TotalActivities = activities.Count,
                TotalStudyHours = 2.5,
                TotalDownloads = 3,
                TotalUploads = 1,
                ThisWeekActivities = 5
            };

            weeklyActivity = new List<WeeklyActivity>
        {
            new() { Day = "Mon", Count = 5, Percentage = 25 },
            new() { Day = "Tue", Count = 8, Percentage = 40 },
            new() { Day = "Wed", Count = 12, Percentage = 60 },
            new() { Day = "Thu", Count = 6, Percentage = 30 },
            new() { Day = "Fri", Count = 10, Percentage = 50 },
            new() { Day = "Sat", Count = 15, Percentage = 75 },
            new() { Day = "Sun", Count = 4, Percentage = 20 }
        };

            topSubjects = new List<SubjectStats>
        {
            new() { Name = "Physics", Count = 12 },
            new() { Name = "Mathematics", Count = 10 },
            new() { Name = "Chemistry", Count = 8 },
            new() { Name = "Biology", Count = 6 },
            new() { Name = "English", Count = 4 }
        };

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = activities.AsEnumerable();

            if (!string.IsNullOrEmpty(selectedActivityType))
            {
                filtered = filtered.Where(a => a.Type == selectedActivityType);
            }

            if (!string.IsNullOrEmpty(selectedDateRange))
            {
                var now = DateTime.Now;
                filtered = selectedDateRange switch
                {
                    "today" => filtered.Where(a => a.Timestamp.Date == now.Date),
                    "week" => filtered.Where(a => a.Timestamp >= now.AddDays(-7)),
                    "month" => filtered.Where(a => a.Timestamp >= now.AddMonths(-1)),
                    "year" => filtered.Where(a => a.Timestamp >= now.AddYears(-1)),
                    _ => filtered
                };
            }

            filtered = sortBy switch
            {
                "date" => filtered.OrderByDescending(a => a.Timestamp),
                "type" => filtered.OrderBy(a => a.Type),
                "duration" => filtered.OrderByDescending(a => a.Duration),
                _ => filtered.OrderByDescending(a => a.Timestamp)
            };

            filteredActivities = filtered.ToList();
            currentPage = 1;
            totalPages = (int)Math.Ceiling((double)filteredActivities.Count / pageSize);
            UpdatePaginatedActivities();
        }

        private void UpdatePaginatedActivities()
        {
            var skip = (currentPage - 1) * pageSize;
            paginatedActivities = filteredActivities.Skip(skip).Take(pageSize).ToList();
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePaginatedActivities();
            }
        }

        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdatePaginatedActivities();
            }
        }

        private void ExportHistory()
        {
            Console.WriteLine("Exporting history...");
            // Implement export functionality
        }

        private void ClearHistory()
        {
            Console.WriteLine("Clearing history...");
            // Implement clear history functionality
        }

        private void ViewActivityDetails(ActivityItem activity)
        {
            Console.WriteLine($"Viewing details for activity: {activity.Title}");
            // Show activity details modal or navigate to details page
        }

        private string GetActivityIconColor(string type)
        {
            return type switch
            {
                "download" => "bg-blue-500",
                "upload" => "bg-green-500",
                "study" => "bg-purple-500",
                "search" => "bg-yellow-500",
                "login" => "bg-gray-500",
                _ => "bg-gray-500"
            };
        }

        private string GetActivityBadgeColor(string type)
        {
            return type switch
            {
                "download" => "bg-blue-100 text-blue-800",
                "upload" => "bg-green-100 text-green-800",
                "study" => "bg-purple-100 text-purple-800",
                "search" => "bg-yellow-100 text-yellow-800",
                "login" => "bg-gray-100 text-gray-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

        public class ActivityItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = "";
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public DateTime Timestamp { get; set; }
            public TimeSpan? Duration { get; set; }
            public string? FileSize { get; set; }
        }

        public class ActivitySummary
        {
            public int TotalActivities { get; set; }
            public double TotalStudyHours { get; set; }
            public int TotalDownloads { get; set; }
            public int TotalUploads { get; set; }
            public int ThisWeekActivities { get; set; }
        }

        public class WeeklyActivity
        {
            public string Day { get; set; } = "";
            public int Count { get; set; }
            public double Percentage { get; set; }
        }

        public class SubjectStats
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }
    }
}