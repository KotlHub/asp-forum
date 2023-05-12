using Microsoft.AspNetCore.Mvc;

namespace ASP_201.Models.Forum
{
    public class ForumSectionViewModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string AuthorAvatarUrl { get; set; } = null!;
        public string UrlIdString { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
