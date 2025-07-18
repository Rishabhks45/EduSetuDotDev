using EduSetu.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class Notes
    {
        private string selectedCategory = "";
        private string searchQuery = "";
        private string viewMode = "grid";
        private string sortBy = "newest";
        private string sortOrder = "desc";
        private int currentPage = 1;
        private const int itemsPerPage = 12;
        private bool showFilters = false;

        private Dictionary<string, string> filters = new()
        {
            ["board"] = "",
            ["university"] = "",
            ["exam"] = "",
            ["state"] = "",
            ["subject"] = "",
            ["course"] = "",
            ["semester"] = "",
            ["noteType"] = "",
            ["difficulty"] = "",
            ["author"] = "",
            ["rating"] = "",
            ["verified"] = "",
            ["premium"] = ""
        };

        private readonly Dictionary<string, List<string>> filterOptions = new()
        {
            ["board"] = new List<string> { "CBSE", "ICSE", "Maharashtra State Board", "Karnataka State Board", "Tamil Nadu State Board" },
            ["university"] = new List<string> { "Delhi University", "Mumbai University", "Bangalore University", "IIT Bombay", "IIT Delhi", "IIT Madras" },
            ["exam"] = new List<string> { "JEE Main", "JEE Advanced", "NEET", "GATE", "CAT", "UPSC", "SSC" },
            ["state"] = new List<string> { "Delhi", "Maharashtra", "Karnataka", "Tamil Nadu", "Gujarat", "Rajasthan" },
            ["subject"] = new List<string> { "Physics", "Chemistry", "Mathematics", "Biology", "Computer Science", "English" },
            ["course"] = new List<string> { "Class 10", "Class 11", "Class 12", "B.Tech Computer Science", "B.Tech Engineering", "JEE Preparation", "NEET Preparation", "GATE Preparation" },
            ["semester"] = new List<string> { "Semester 1", "Semester 2", "Semester 3", "Semester 4", "Semester 5", "Semester 6", "Semester 7", "Semester 8" },
            ["noteType"] = new List<string> { "lecture", "summary", "reference", "assignment", "lab" },
            ["difficulty"] = new List<string> { "beginner", "intermediate", "advanced" },
            ["author"] = new List<string> { "Dr. Sarah Johnson", "Prof. Michael Chen", "Dr. Priya Sharma", "Prof. Rajesh Kumar", "Dr. Alex Thompson", "Ms. Lisa Anderson", "Prof. David Wilson", "Dr. Emily Rodriguez" },
            ["rating"] = new List<string> { "4+ Stars", "3+ Stars", "2+ Stars", "1+ Stars" }
        };

        private List<Note> notes = new()
    {
        new Note
        {
            Id = "1",
            Title = "CBSE Class 12 Physics Complete Notes",
            Subject = "Physics",
            Course = "Class 12",
            Semester = "",
            University = "",
            Board = "CBSE",
            Exam = "",
            State = "",
            Author = "Dr. Sarah Johnson",
            Description = "Comprehensive physics notes covering all chapters for CBSE Class 12 including mechanics, thermodynamics, optics, and modern physics with solved examples.",
            Tags = new List<string> { "Physics", "CBSE", "Class 12", "Mechanics", "Thermodynamics", "Optics" },
            DownloadCount = 5245,
            ViewCount = 28750,
            Rating = 4.8f,
            TotalRatings = 256,
            FileSize = "15.2 MB",
            Pages = 345,
            UploadDate = "2024-01-15",
            LastUpdated = "2024-01-20",
            Thumbnail = "https://images.pexels.com/photos/4164418/pexels-photo-4164418.jpeg",
            Category = "board",
            NoteType = "lecture",
            Difficulty = "intermediate",
            IsVerified = true,
            IsPremium = false
        },
        new Note
        {
            Id = "2",
            Title = "Delhi University Computer Science Semester 4 Notes",
            Subject = "Computer Science",
            Course = "B.Tech Computer Science",
            Semester = "Semester 4",
            University = "Delhi University",
            Board = "",
            Exam = "",
            State = "Delhi",
            Author = "Prof. Michael Chen",
            Description = "Complete study material for DU Computer Science covering Data Structures, Algorithms, Database Management, and Operating Systems.",
            Tags = new List<string> { "Computer Science", "Data Structures", "Algorithms", "DBMS", "Operating Systems" },
            DownloadCount = 3890,
            ViewCount = 18420,
            Rating = 4.9f,
            TotalRatings = 203,
            FileSize = "22.1 MB",
            Pages = 420,
            UploadDate = "2024-01-10",
            LastUpdated = "2024-01-18",
            Thumbnail = "https://images.pexels.com/photos/3861969/pexels-photo-3861969.jpeg",
            Category = "university",
            NoteType = "lecture",
            Difficulty = "advanced",
            IsVerified = true,
            IsPremium = true
        }
    };

        // Add more sample notes for pagination demonstration
        protected override void OnInitialized()
        {
            for (int i = 3; i <= 35; i++)
            {
                var subjects = new[] { "Physics", "Chemistry", "Mathematics", "Biology", "Computer Science" };
                var universities = new[] { "Delhi University", "Mumbai University", "Bangalore University" };
                var boards = new[] { "CBSE", "ICSE", "State Board" };
                var exams = new[] { "JEE", "NEET", "GATE" };
                var states = new[] { "Delhi", "Maharashtra", "Karnataka" };
                var categories = new[] { "board", "university", "exam", "state" };
                var noteTypes = new[] { "lecture", "summary", "reference", "assignment", "lab" };
                var difficulties = new[] { "beginner", "intermediate", "advanced" };

                notes.Add(new Note
                {
                    Id = i.ToString(),
                    Title = $"Sample Note {i}",
                    Subject = subjects[i % subjects.Length],
                    Course = "Sample Course",
                    Semester = $"Semester {(i % 8) + 1}",
                    University = universities[i % universities.Length],
                    Board = boards[i % boards.Length],
                    Exam = exams[i % exams.Length],
                    State = states[i % states.Length],
                    Author = $"Author {i}",
                    Description = $"Sample description for note {i}",
                    Tags = new List<string> { "Sample", "Education", "Learning" },
                    DownloadCount = Random.Shared.Next(100, 5000),
                    ViewCount = Random.Shared.Next(1000, 20000),
                    Rating = 4.0f + (float)Random.Shared.NextDouble(),
                    TotalRatings = Random.Shared.Next(50, 300),
                    FileSize = $"{Random.Shared.Next(5, 20)}.{Random.Shared.Next(0, 9)} MB",
                    Pages = Random.Shared.Next(50, 400),
                    UploadDate = "2024-01-01",
                    LastUpdated = "2024-01-01",
                    Thumbnail = "https://images.pexels.com/photos/4164418/pexels-photo-4164418.jpeg",
                    Category = categories[i % categories.Length],
                    NoteType = noteTypes[i % noteTypes.Length],
                    Difficulty = difficulties[i % difficulties.Length],
                    IsVerified = Random.Shared.Next(2) == 1,
                    IsPremium = Random.Shared.Next(10) > 7
                });
            }
        }

        private List<Note> filteredNotes => notes.Where(note =>
        {
            var matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                               note.Title.ToLower().Contains(searchQuery.ToLower()) ||
                               note.Description.ToLower().Contains(searchQuery.ToLower()) ||
                               note.Tags.Any(tag => tag.ToLower().Contains(searchQuery.ToLower())) ||
                               note.Author.ToLower().Contains(searchQuery.ToLower());

            var matchesCategory = string.IsNullOrEmpty(selectedCategory) || note.Category == selectedCategory;

            var matchesFilters = filters.All(filter =>
            {
                if (string.IsNullOrEmpty(filter.Value)) return true;

                return filter.Key switch
                {
                    "rating" => note.Rating >= int.Parse(filter.Value[0].ToString()),
                    "verified" => !bool.Parse(filter.Value) || note.IsVerified,
                    "premium" => !bool.Parse(filter.Value) || note.IsPremium,
                    _ => (GetNoteValue(note, filter.Key) ?? "") == (filter.Value ?? "")
                };
            });

            return matchesSearch && matchesCategory && matchesFilters;
        }).ToList();

        private string GetNoteValue(Note note, string filterKey)
        {
            return filterKey switch
            {
                "board" => note.Board,
                "university" => note.University,
                "exam" => note.Exam,
                "state" => note.State,
                "subject" => note.Subject,
                "course" => note.Course,
                "semester" => note.Semester,
                "noteType" => note.NoteType,
                "difficulty" => note.Difficulty,
                "author" => note.Author,
                _ => ""
            };
        }

        private List<Note> sortedNotes
        {
            get
            {
                return sortBy switch
                {
                    "newest" => filteredNotes
                        .OrderByDescending(note => DateTime.Parse(note.UploadDate))
                        .ThenBy(note => note.Title)
                        .ToList(),

                    "popular" => filteredNotes
                        .OrderByDescending(note => note.ViewCount)
                        .ThenBy(note => note.Title)
                        .ToList(),

                    "rating" => filteredNotes
                        .OrderByDescending(note => note.Rating)
                        .ThenBy(note => note.Title)
                        .ToList(),

                    "downloads" => filteredNotes
                        .OrderByDescending(note => note.DownloadCount)
                        .ThenBy(note => note.Title)
                        .ToList(),

                    _ => filteredNotes
                        .OrderByDescending(note => DateTime.Parse(note.UploadDate))
                        .ThenBy(note => note.Title)
                        .ToList(),
                };
            }
        }

        private int totalPages => (int)Math.Ceiling((double)filteredNotes.Count / itemsPerPage);
        private int startIndex => (currentPage - 1) * itemsPerPage;
        private int endIndex => startIndex + itemsPerPage;
        private List<Note> currentNotes => sortedNotes.Skip(startIndex).Take(itemsPerPage).ToList();

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

        private void ToggleShowFilters()
        {
            showFilters = !showFilters;
        }

        private void ToggleSortOrder()
        {
            sortOrder = sortOrder == "asc" ? "desc" : "asc";
            if (sortOrder == "desc")
            {
                // Reverse the sorted list for descending order
                sortedNotes.Reverse();
            }
        }

        private void ClearFilters()
        {
            filters = new Dictionary<string, string>
            {
                ["board"] = "",
                ["university"] = "",
                ["exam"] = "",
                ["state"] = "",
                ["subject"] = "",
                ["course"] = "",
                ["semester"] = "",
                ["noteType"] = "",
                ["difficulty"] = "",
                ["author"] = "",
                ["rating"] = "",
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

        private void ClearFilter(string filterType)
        {
            if (filters.ContainsKey(filterType))
            {
                filters[filterType] = "";
            }
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

        private List<string> GetActiveFilters()
        {
            return selectedCategory switch
            {
                "board" => new List<string> { "board", "subject", "course" },
                "university" => new List<string> { "university", "subject", "course", "semester" },
                "exam" => new List<string> { "exam", "subject", "course" },
                "state" => new List<string> { "state", "subject", "course" },
                _ => new List<string>()
            };
        }

        private string GetFilterLabel(string filterType)
        {
            return filterType switch
            {
                "board" => "Board",
                "university" => "University",
                "exam" => "Exam",
                "state" => "State",
                "subject" => "Subject",
                "course" => "Course",
                "semester" => "Semester",
                "noteType" => "Note Type",
                "difficulty" => "Difficulty",
                "author" => "Author",
                "rating" => "Rating",
                _ => filterType
            };
        }

        private void HandleViewNote(Note note)
        {
            NavigationManager.NavigateTo($"/notes/{note.Id}");
        }

        private void HandleDownloadNote(Note note)
        {
            Console.WriteLine($"Downloading note: {note.Id}");
            // In a real app, this would trigger the download
        }
    }
}