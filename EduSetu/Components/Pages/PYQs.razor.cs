using EduSetu.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class PYQs
    {
        private string selectedCategory = "";
        private string searchQuery = "";
        private Dictionary<string, string> filters = new()
        {
            ["board"] = "",
            ["university"] = "",
            ["exam"] = "",
            ["state"] = "",
            ["class"] = "",
            ["course"] = "",
            ["year"] = ""
        };

        private readonly List<Category> categories = new()
    {
        new Category { Id = "board", Name = "Board Wise" },
        new Category { Id = "university", Name = "University Wise" },
        new Category { Id = "exam", Name = "Exam Wise" },
        new Category { Id = "state", Name = "State Wise" }
    };

        private readonly Dictionary<string, List<string>> filterOptions = new()
        {
            ["board"] = new List<string> { "CBSE", "ICSE", "State Board", "NIOS", "IB" },
            ["university"] = new List<string> { "Delhi University", "Mumbai University", "Bangalore University", "Pune University", "Chennai University" },
            ["exam"] = new List<string> { "JEE Main", "JEE Advanced", "NEET", "GATE", "CAT", "UPSC", "SSC" },
            ["state"] = new List<string> { "Delhi", "Maharashtra", "Karnataka", "Tamil Nadu", "Gujarat", "Rajasthan" },
            ["class"] = new List<string> { "Class 10", "Class 11", "Class 12" },
            ["course"] = new List<string> { "Engineering", "Medical", "Arts", "Commerce", "Science" },
            ["year"] = new List<string> { "2024", "2023", "2022", "2021", "2020", "2019" }
        };

        private readonly List<Subject> subjects = new()
    {
        new Subject
        {
            Code = "CS101",
            Name = "Introduction to Computer Science",
            University = "Delhi University",
            Years = new List<string> { "2023", "2022", "2021" },
            Category = "university",
            Board = "",
            Exam = "",
            State = "Delhi"
        },
        new Subject
        {
            Code = "MTH201",
            Name = "Advanced Mathematics",
            University = "Mumbai University",
            Years = new List<string> { "2023", "2022", "2021", "2020" },
            Category = "university",
            Board = "",
            Exam = "",
            State = "Maharashtra"
        },
        new Subject
        {
            Code = "PHY301",
            Name = "Quantum Physics",
            University = "IIT Bombay",
            Years = new List<string> { "2023", "2022" },
            Category = "university",
            Board = "",
            Exam = "JEE Advanced",
            State = "Maharashtra"
        },
        new Subject
        {
            Code = "CBSE-PHY-12",
            Name = "Physics Class 12",
            University = "",
            Years = new List<string> { "2024", "2023", "2022" },
            Category = "board",
            Board = "CBSE",
            Exam = "",
            State = ""
        },
        new Subject
        {
            Code = "NEET-BIO",
            Name = "Biology for NEET",
            University = "",
            Years = new List<string> { "2024", "2023", "2022", "2021" },
            Category = "exam",
            Board = "",
            Exam = "NEET",
            State = ""
        }
    };

        private List<Subject> filteredSubjects => subjects.Where(subject =>
        {
            var matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                               subject.Name.ToLower().Contains(searchQuery.ToLower()) ||
                               subject.Code.ToLower().Contains(searchQuery.ToLower());

            var matchesCategory = string.IsNullOrEmpty(selectedCategory) || subject.Category == selectedCategory;

            var matchesFilters = filters.All(filter =>
            {
                if (string.IsNullOrEmpty(filter.Value)) return true;
                return GetSubjectValue(subject, filter.Key) == filter.Value;
            });

            return matchesSearch && matchesCategory && matchesFilters;
        }).ToList();

        private string GetSubjectValue(Subject subject, string filterKey)
        {
            return filterKey switch
            {
                "board" => subject.Board,
                "university" => subject.University,
                "exam" => subject.Exam,
                "state" => subject.State,
                _ => ""
            };
        }

        private void SelectCategory(string category)
        {
            selectedCategory = selectedCategory == category ? "" : category;
            ClearFilters();
        }

        private void ClearCategorySelection()
        {
            selectedCategory = "";
            ClearFilters();
        }

        private void ClearFilters()
        {
            filters = new Dictionary<string, string>
            {
                ["board"] = "",
                ["university"] = "",
                ["exam"] = "",
                ["state"] = "",
                ["class"] = "",
                ["course"] = "",
                ["year"] = ""
            };
        }

        private void ClearFilter(string filterType)
        {
            if (filters.ContainsKey(filterType))
            {
                filters[filterType] = "";
            }
        }

        private List<string> GetActiveFilters()
        {
            return selectedCategory switch
            {
                "board" => new List<string> { "board", "class", "year" },
                "university" => new List<string> { "university", "course", "year" },
                "exam" => new List<string> { "exam", "year" },
                "state" => new List<string> { "state", "year" },
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
                "class" => "Class",
                "course" => "Course",
                "year" => "Year",
                _ => filterType
            };
        }

        private void HandleViewPapers(Subject subject)
        {
            var paperId = $"{subject.Code.ToLower()}-2023-{subject.Category}";
            NavigationManager.NavigateTo($"/papers/{paperId}");
        }

        public class Category
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
        }
    }
}