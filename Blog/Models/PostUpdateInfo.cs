namespace Blog.Models
{
    public sealed class PostUpdateInfo
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string[] Tags { get; set; }
    }
}
