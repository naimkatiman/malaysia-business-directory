using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MalaysiaBusinessDirectory.Api.Models
{
    public class BusinessAnalytics
    {
        [Key]
        public Guid Id { get; set; }

        // Foreign key for Business
        public Guid BusinessId { get; set; }
        
        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }

        // Analytics metrics
        public int TotalViews { get; set; } = 0;
        public int WeeklyViews { get; set; } = 0;
        public int MonthlyViews { get; set; } = 0;
        public int TotalBookmarks { get; set; } = 0;
        public int TotalClicks { get; set; } = 0;
        
        // Engagement rates (stored as percentages)
        public decimal ClickThroughRate { get; set; } = 0;
        public decimal BookmarkRate { get; set; } = 0;
        
        // Time metrics
        public double AverageVisitDurationSeconds { get; set; } = 0;
        
        // Demographic information (stored as JSON string)
        [MaxLength(2000)]
        public string? UserDemographics { get; set; }
        
        // Traffic sources (stored as JSON string)
        [MaxLength(2000)]
        public string? TrafficSources { get; set; }

        // Timestamp fields
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
