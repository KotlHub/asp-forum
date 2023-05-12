using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace ASP_201.Models.Forum
{
    public class ForumSectionFormModel
    {
        [FromForm(Name = "section-title")]
        public string Title { get; set; } = null!;
        [FromForm(Name = "section-description")]
        public string Description { get; set; } = null!;
        
        public string SectionId { get; set; } = null!;
    }
}
