using Microsoft.AspNetCore.Components;

namespace EduSetu.Components.Pages
{
    public partial class VideoView
    {
        [Parameter]
        public string? id { get; set; }

        private VideoModel? video;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await Task.Delay(500); // Simulate loading
            video = GetVideoById(id);
            isLoading = false;
        }

        private VideoModel? GetVideoById(string? videoId)
        {
            // Simulate fetching from a data source
            var videos = new List<VideoModel>
        {
            new VideoModel { Id = "2", Title = "Machine Learning Fundamentals Video Course", Author = "Prof. Michael Chen", Subject = "Computer Science", Course = "B.Tech Computer Science", Description = "Complete video course covering machine learning algorithms, neural networks, and practical implementations.", Rating = 4.9, UploadDate = DateTime.Parse("2024-01-10"), VideoUrl = "https://www.w3schools.com/html/mov_bbb.mp4" },
            new VideoModel { Id = "4", Title = "Mathematics Calculus Tutorial Series", Author = "Dr. Priya Sharma", Subject = "Mathematics", Course = "Class 12", Description = "Step-by-step calculus tutorials covering limits, derivatives, and integrals with solved examples.", Rating = 4.6, UploadDate = DateTime.Parse("2024-01-12"), VideoUrl = "https://www.w3schools.com/html/movie.mp4" }
        };
            return videos.FirstOrDefault(v => v.Id == videoId);
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

        public class VideoModel
        {
            public string Id { get; set; } = "";
            public string Title { get; set; } = "";
            public string Author { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Course { get; set; } = "";
            public string Description { get; set; } = "";
            public double Rating { get; set; }
            public DateTime UploadDate { get; set; }
            public string VideoUrl { get; set; } = "";
        }
    }
}