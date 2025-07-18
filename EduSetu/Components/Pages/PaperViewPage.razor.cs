using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class PaperViewPage
    {
        [Parameter]
        public int? PaperId { get; set; }

        private List<Paper> papers = new();
        private List<Paper> filteredPapers = new();
        private List<Paper> relatedPapers = new();
        private Paper? selectedPaper;
        private bool showSearch = false;
        private string searchQuery = "";
        private string selectedSubject = "";
        private string selectedType = "";
        private int currentPage = 1;
        private int totalPages = 1;
        private int pageSize = 12;

        protected override async Task OnInitializedAsync()
        {
            await LoadPapers();

            if (PaperId.HasValue)
            {
                selectedPaper = papers.FirstOrDefault(p => p.Id == PaperId.Value);
                if (selectedPaper != null)
                {
                    LoadRelatedPapers();
                }
            }
        }

        private async Task LoadPapers()
        {
            papers = new List<Paper>
        {
            new() {
                Id = 1,
                Title = "Physics Class 12 Question Paper 2023",
                Description = "Complete question paper with solutions for Class 12 Physics board examination.",
                Subject = "physics",
                Type = "question-paper",
                Author = "Dr. Sharma",
                UploadDate = DateTime.Now.AddDays(-5),
                Downloads = 234,
                Rating = 4.8,
                FileSize = "2.4 MB",
                Pages = 8,
                Format = "PDF",
                Language = "English",
                Tags = new List<string> { "Class 12", "Board Exam", "2023" }
            },
            new() {
                Id = 2,
                Title = "Chemistry Lab Manual",
                Description = "Comprehensive lab manual with experiments and procedures for chemistry practicals.",
                Subject = "chemistry",
                Type = "study-material",
                Author = "Prof. Gupta",
                UploadDate = DateTime.Now.AddDays(-7),
                Downloads = 156,
                Rating = 4.6,
                FileSize = "1.8 MB",
                Pages = 45,
                Format = "PDF",
                Language = "English",
                Tags = new List<string> { "Lab Manual", "Practical", "Experiments" }
            },
            new() {
                Id = 3,
                Title = "Mathematics Integration Notes",
                Description = "Detailed notes on integration techniques with solved examples and practice problems.",
                Subject = "mathematics",
                Type = "notes",
                Author = "Mr. Kumar",
                UploadDate = DateTime.Now.AddDays(-3),
                Downloads = 189,
                Rating = 4.9,
                FileSize = "3.1 MB",
                Pages = 32,
                Format = "PDF",
                Language = "English",
                Tags = new List<string> { "Integration", "Calculus", "Class 12" }
            },
            new() {
                Id = 4,
                Title = "Biology Cell Division Solutions",
                Description = "Step-by-step solutions for cell division problems with diagrams and explanations.",
                Subject = "biology",
                Type = "solutions",
                Author = "Dr. Patel",
                UploadDate = DateTime.Now.AddDays(-10),
                Downloads = 98,
                Rating = 4.7,
                FileSize = "1.5 MB",
                Pages = 15,
                Format = "PDF",
                Language = "English",
                Tags = new List<string> { "Cell Division", "Mitosis", "Meiosis" }
            }
        };

            ApplyFilters();
        }

        private void ShowSearch()
        {
            showSearch = !showSearch;
        }

        private void HandleSearch()
        {
            ApplyFilters();
        }

        private void ClearSearch()
        {
            searchQuery = "";
            selectedSubject = "";
            selectedType = "";
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = papers.AsEnumerable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                filtered = filtered.Where(p =>
                    p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrEmpty(selectedSubject))
            {
                filtered = filtered.Where(p => p.Subject == selectedSubject);
            }

            if (!string.IsNullOrEmpty(selectedType))
            {
                filtered = filtered.Where(p => p.Type == selectedType);
            }

            filteredPapers = filtered.ToList();
            currentPage = 1;
            totalPages = (int)Math.Ceiling((double)filteredPapers.Count / pageSize);
        }

        private void SelectPaper(Paper paper)
        {
            selectedPaper = paper;
            LoadRelatedPapers();
            NavigationManager.NavigateTo($"/papers/{paper.Id}");
        }

        private void LoadRelatedPapers()
        {
            if (selectedPaper != null)
            {
                relatedPapers = papers
                    .Where(p => p.Id != selectedPaper.Id && (p.Subject == selectedPaper.Subject || p.Type == selectedPaper.Type))
                    .Take(3)
                    .ToList();
            }
        }

        private void DownloadPaper()
        {
            if (selectedPaper != null)
            {
                Console.WriteLine($"Downloading paper: {selectedPaper.Title}");
                // Simulate download
            }
        }

        private void SharePaper()
        {
            if (selectedPaper != null)
            {
                Console.WriteLine($"Sharing paper: {selectedPaper.Title}");
                // Implement share functionality
            }
        }

        private void OpenPaperViewer()
        {
            if (selectedPaper != null)
            {
                Console.WriteLine($"Opening paper viewer for: {selectedPaper.Title}");
                // Open full paper viewer
            }
        }

        private void ShowPaperMenu(Paper paper)
        {
            Console.WriteLine($"Show menu for paper: {paper.Title}");
            // Show context menu
        }

        private void UploadPaper()
        {
            NavigationManager.NavigateTo("/profile/uploads");
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

        private string GetTypeBadgeColor(string type)
        {
            return type switch
            {
                "question-paper" => "bg-red-100 text-red-800",
                "notes" => "bg-blue-100 text-blue-800",
                "solutions" => "bg-green-100 text-green-800",
                "study-material" => "bg-purple-100 text-purple-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

        private string GetSubjectBadgeColor(string subject)
        {
            return subject switch
            {
                "physics" => "bg-indigo-100 text-indigo-800",
                "chemistry" => "bg-yellow-100 text-yellow-800",
                "mathematics" => "bg-red-100 text-red-800",
                "biology" => "bg-green-100 text-green-800",
                "english" => "bg-blue-100 text-blue-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }

        public class Paper
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Type { get; set; } = "";
            public string Author { get; set; } = "";
            public DateTime UploadDate { get; set; }
            public int Downloads { get; set; }
            public double Rating { get; set; }
            public string FileSize { get; set; } = "";
            public int Pages { get; set; }
            public string Format { get; set; } = "";
            public string Language { get; set; } = "";
            public List<string> Tags { get; set; } = new();
        }
    }
}