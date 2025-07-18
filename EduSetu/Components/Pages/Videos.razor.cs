using EduSetu.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class Videos
    {
        private string selectedCategory = "";
        private string searchQuery = "";
        private string viewMode = "grid";
        private string sortBy = "newest";
        private int currentPage = 1;
        private const int itemsPerPage = 12;

        private Dictionary<string, string> filters = new()
        {
            ["subject"] = "",
            ["instructor"] = "",
            ["level"] = "",
            ["duration"] = "",
            ["language"] = "",
            ["quality"] = "",
            ["verified"] = "",
            ["premium"] = ""
        };

        private readonly Dictionary<string, List<string>> filterOptions = new()
        {
            ["subject"] = new List<string> { "Physics", "Chemistry", "Mathematics", "Biology", "Computer Science", "English", "History", "Geography" },
            ["instructor"] = new List<string> { "Dr. Sarah Johnson", "Prof. Michael Chen", "Dr. Priya Sharma", "Prof. Rajesh Kumar", "Dr. Alex Thompson" },
            ["level"] = new List<string> { "Beginner", "Intermediate", "Advanced", "Expert" },
            ["duration"] = new List<string> { "Short (< 10 min)", "Medium (10-30 min)", "Long (30+ min)" },
            ["language"] = new List<string> { "English", "Hindi", "Tamil", "Telugu", "Bengali" },
            ["quality"] = new List<string> { "HD", "Full HD", "4K" }
        };

        private List<Video> videos = new()
    {
        new Video
        {
            Id = "1",
            Title = "Complete Physics Class 12 - Mechanics",
            Subject = "Physics",
            Instructor = "Dr. Sarah Johnson",
            Description = "Comprehensive video lecture covering all mechanics topics for Class 12 including Newton's laws, momentum, and energy conservation.",
            Duration = "45:30",
            Views = 15420,
            Rating = 4.8f,
            TotalRatings = 342,
            Thumbnail = "https://images.pexels.com/photos/4164418/pexels-photo-4164418.jpeg",
            VideoUrl = "https://example.com/video1.mp4",
            Category = "physics",
            Level = "Intermediate",
            Language = "English",
            Quality = "HD",
            IsVerified = true,
            IsPremium = false,
            UploadDate = "2024-01-15"
        },
        new Video
        {
            Id = "2",
            Title = "Data Structures and Algorithms - Part 1",
            Subject = "Computer Science",
            Instructor = "Prof. Michael Chen",
            Description = "Introduction to fundamental data structures including arrays, linked lists, and basic algorithms.",
            Duration = "32:15",
            Views = 8920,
            Rating = 4.9f,
            TotalRatings = 156,
            Thumbnail = "https://images.pexels.com/photos/3861969/pexels-photo-3861969.jpeg",
            VideoUrl = "https://example.com/video2.mp4",
            Category = "computer-science",
            Level = "Advanced",
            Language = "English",
            Quality = "Full HD",
            IsVerified = true,
            IsPremium = true,
            UploadDate = "2024-01-10"
        }
    };

        // Add more sample videos
        protected override void OnInitialized()
        {
            for (int i = 3; i <= 25; i++)
            {
                var subjects = new[] { "Physics", "Chemistry", "Mathematics", "Biology", "Computer Science" };
                var instructors = new[] { "Dr. Sarah Johnson", "Prof. Michael Chen", "Dr. Priya Sharma", "Prof. Rajesh Kumar" };
                var levels = new[] { "Beginner", "Intermediate", "Advanced" };
                var languages = new[] { "English", "Hindi" };
                var qualities = new[] { "HD", "Full HD" };

                videos.Add(new Video
                {
                    Id = i.ToString(),
                    Title = $"Sample Video {i}",
                    Subject = subjects[i % subjects.Length],
                    Instructor = instructors[i % instructors.Length],
                    Description = $"Sample description for video {i}",
                    Duration = $"{Random.Shared.Next(10, 60)}:{Random.Shared.Next(0, 59):D2}",
                    Views = Random.Shared.Next(1000, 20000),
                    Rating = 4.0f + (float)Random.Shared.NextDouble(),
                    TotalRatings = Random.Shared.Next(50, 300),
                    Thumbnail = "https://images.pexels.com/photos/4164418/pexels-photo-4164418.jpeg",
                    VideoUrl = $"https://example.com/video{i}.mp4",
                    Category = "sample",
                    Level = levels[i % levels.Length],
                    Language = languages[i % languages.Length],
                    Quality = qualities[i % qualities.Length],
                    IsVerified = Random.Shared.Next(2) == 1,
                    IsPremium = Random.Shared.Next(10) > 7,
                    UploadDate = "2024-01-01"
                });
            }
        }

        private List<Video> filteredVideos => videos.Where(video =>
        {
            var matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                               video.Title.ToLower().Contains(searchQuery.ToLower()) ||
                               video.Description.ToLower().Contains(searchQuery.ToLower()) ||
                               video.Instructor.ToLower().Contains(searchQuery.ToLower());

            var matchesCategory = string.IsNullOrEmpty(selectedCategory) || video.Category == selectedCategory;

            var matchesFilters = filters.All(filter =>
            {
                if (string.IsNullOrEmpty(filter.Value)) return true;

                return filter.Key switch
                {
                    "verified" => !bool.Parse(filter.Value) || video.IsVerified,
                    "premium" => !bool.Parse(filter.Value) || video.IsPremium,
                    _ => (GetVideoValue(video, filter.Key) ?? "") == (filter.Value ?? "")
                };
            });

            return matchesSearch && matchesCategory && matchesFilters;
        }).ToList();

        private string GetVideoValue(Video video, string filterKey)
        {
            return filterKey switch
            {
                "subject" => video.Subject,
                "instructor" => video.Instructor,
                "level" => video.Level,
                "duration" => video.Duration,
                "language" => video.Language,
                "quality" => video.Quality,
                _ => ""
            };
        }

        private List<Video> sortedVideos
        {
            get
            {
                return sortBy switch
                {
                    "newest" => filteredVideos
                        .OrderByDescending(video => DateTime.Parse(video.UploadDate))
                        .ThenBy(video => video.Title)
                        .ToList(),

                    "popular" => filteredVideos
                        .OrderByDescending(video => video.Views)
                        .ThenBy(video => video.Title)
                        .ToList(),

                    "rating" => filteredVideos
                        .OrderByDescending(video => video.Rating)
                        .ThenBy(video => video.Title)
                        .ToList(),

                    "duration" => filteredVideos
                        .OrderBy(video => ParseDuration(video.Duration))
                        .ThenBy(video => video.Title)
                        .ToList(),

                    _ => filteredVideos
                        .OrderByDescending(video => DateTime.Parse(video.UploadDate))
                        .ThenBy(video => video.Title)
                        .ToList(),
                };
            }
        }

        private int ParseDuration(string duration)
        {
            var parts = duration.Split(":");
            if (parts.Length == 2 && int.TryParse(parts[0], out int minutes) && int.TryParse(parts[1], out int seconds))
            {
                return minutes * 60 + seconds;
            }
            return 0;
        }

        private int totalPages => (int)Math.Ceiling((double)filteredVideos.Count / itemsPerPage);
        private int startIndex => (currentPage - 1) * itemsPerPage;
        private int endIndex => startIndex + itemsPerPage;
        private List<Video> currentVideos => sortedVideos.Skip(startIndex).Take(itemsPerPage).ToList();

        private void SelectCategory(string category)
        {
            selectedCategory = (selectedCategory == category ? "" : category);
            ClearFilters();
            currentPage = 1;
        }

        private void SetViewMode(string mode)
        {
            viewMode = mode;
        }

        private void ClearFilters()
        {
            filters = new Dictionary<string, string>
            {
                ["subject"] = "",
                ["instructor"] = "",
                ["level"] = "",
                ["duration"] = "",
                ["language"] = "",
                ["quality"] = "",
                ["verified"] = "",
                ["premium"] = ""
            };
        }

        private void HandleFilterChange(string filterType, string value)
        {
            if (filters.ContainsKey(filterType))
            {
                filters[filterType] = value;
            }
            currentPage = 1;
        }

        private void HandlePageChange(int page)
        {
            if (page >= 1 && page <= totalPages)
            {
                currentPage = page;
            }
        }

        private List<object> GetPageNumbers()
        {
            var pages = new List<object>();
            var maxVisible = 5;

            if (totalPages <= maxVisible)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    pages.Add(i);
                }
            }
            else
            {
                if (currentPage <= 3)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        pages.Add(i);
                    }
                    pages.Add("...");
                    pages.Add(totalPages);
                }
                else if (currentPage >= totalPages - 2)
                {
                    pages.Add(1);
                    pages.Add("...");
                    for (int i = totalPages - 3; i <= totalPages; i++)
                    {
                        pages.Add(i);
                    }
                }
                else
                {
                    pages.Add(1);
                    pages.Add("...");
                    for (int i = currentPage - 1; i <= currentPage + 1; i++)
                    {
                        pages.Add(i);
                    }
                    pages.Add("...");
                    pages.Add(totalPages);
                }
            }

            return pages;
        }

        private void HandleViewVideo(Video video)
        {
            NavigationManager.NavigateTo($"/videos/{video.Id}");
        }

        private void HandleDownloadVideo(Video video)
        {
            Console.WriteLine($"Downloading video: {video.Id}");
            // In a real app, this would trigger the download
        }


    }
}