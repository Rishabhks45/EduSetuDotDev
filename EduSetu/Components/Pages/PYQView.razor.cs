using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class PYQView
    {
        [Parameter]
        public string? id { get; set; }

        private PYQModel? pyq;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await Task.Delay(500); // Simulate loading
            pyq = GetPYQById(id);
            isLoading = false;
        }

        private PYQModel? GetPYQById(string? pyqId)
        {
            // Simulate fetching from a data source
            var pyqs = new List<PYQModel>
        {
            new PYQModel { Id = "3", Title = "Physics Class 12 Previous Year Questions", Author = "CBSE Board", Subject = "Physics", Course = "Class 12", Description = "Collection of previous year physics questions for Class 12 CBSE board examinations.", Rating = 4.7, UploadDate = DateTime.Parse("2024-01-08") }
        };
            return pyqs.FirstOrDefault(p => p.Id == pyqId);
        }

        private string FormatDate(DateTime date)
        {
            var now = DateTime.Now;
            var diffTime = Math.Abs((now - date).TotalDays);
            var diffDays = (int)Math.Ceiling(diffTime);
            if (diffDays == 1) return "1 day ago";
            if (diffDays < 7) return $"{diffDays} days ago";
            if (diffDays < 30) return $"{Math.Ceiling(diffDays / 7.0)} weeks ago";
            return date.ToShortDateString();
        }

        public class PYQModel
        {
            public string Id { get; set; } = "";
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Course { get; set; } = "";
            public string Description { get; set; } = "";
            public double Rating { get; set; }
            public DateTime UploadDate { get; set; }
        }
    }
}