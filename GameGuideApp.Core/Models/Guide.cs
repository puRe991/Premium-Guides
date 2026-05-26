using System;

namespace GameGuideApp.Core.Models
{
    public class Guide
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public bool IsLocked { get; set; }
    }
}
