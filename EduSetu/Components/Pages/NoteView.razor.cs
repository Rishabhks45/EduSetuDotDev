using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class NoteView
    {
        [Parameter]
        public string? id { get; set; }

        private NoteModel? note;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await Task.Delay(500); // Simulate loading
            note = GetNoteById(id);
            isLoading = false;
        }

        private NoteModel? GetNoteById(string? noteId)
        {
            // Simulate fetching from a data source
            var notes = new List<NoteModel>
        {
            new NoteModel { Id = "1", Title = "Data Structures Complete Notes", Author = "Dr. Sarah Johnson", Subject = "Computer Science", Course = "B.Tech Computer Science", Description = "Comprehensive notes covering all fundamental data structures including arrays, linked lists, stacks, queues, trees, and graphs.", Rating = 4.8, UploadDate = DateTime.Parse("2024-01-15") },
            new NoteModel { Id = "5", Title = "Chemistry Organic Compounds Notes", Author = "Dr. Rajesh Kumar", Subject = "Chemistry", Course = "Class 12", Description = "Detailed notes on organic chemistry covering hydrocarbons, alcohols, aldehydes, and ketones.", Rating = 4.5, UploadDate = DateTime.Parse("2024-01-14") },
            new NoteModel { Id = "6", Title = "Biology NEET Preparation Guide", Author = "Dr. Anita Verma", Subject = "Biology", Course = "NEET Preparation", Description = "Comprehensive biology preparation guide for NEET with important topics and practice questions.", Rating = 4.8, UploadDate = DateTime.Parse("2024-01-05") }
        };
            return notes.FirstOrDefault(n => n.Id == noteId);
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

        public class NoteModel
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