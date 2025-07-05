namespace EduSetu.Components.Shared
{
    public class Note
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Course { get; set; } = "";
        public string Semester { get; set; } = "";
        public string University { get; set; } = "";
        public string Board { get; set; } = "";
        public string Exam { get; set; } = "";
        public string State { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public int DownloadCount { get; set; }
        public int ViewCount { get; set; }
        public float Rating { get; set; }
        public int TotalRatings { get; set; }
        public string FileSize { get; set; } = "";
        public int Pages { get; set; }
        public string UploadDate { get; set; } = "";
        public string LastUpdated { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string Category { get; set; } = "";
        public string NoteType { get; set; } = "";
        public string Difficulty { get; set; } = "";
        public bool IsVerified { get; set; }
        public bool IsPremium { get; set; }
    }

    public class Video
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Instructor { get; set; } = "";
        public string Description { get; set; } = "";
        public string Duration { get; set; } = "";
        public int Views { get; set; }
        public float Rating { get; set; }
        public int TotalRatings { get; set; }
        public string Thumbnail { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public string Category { get; set; } = "";
        public string Level { get; set; } = "";
        public string Language { get; set; } = "";
        public string Quality { get; set; } = "";
        public bool IsVerified { get; set; }
        public bool IsPremium { get; set; }
        public string UploadDate { get; set; } = "";
    }

    public class Subject
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string University { get; set; } = "";
        public string Board { get; set; } = "";
        public string Exam { get; set; } = "";
        public string State { get; set; } = "";
        public List<string> Years { get; set; } = new();
    }
} 