using System;

namespace MalaysiaBusinessDirectory.Api.DTOs
{
    public class BusinessAnalyticsDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int TotalViews { get; set; }
        public int WeeklyViews { get; set; }
        public int MonthlyViews { get; set; }
        public int TotalBookmarks { get; set; }
        public int TotalClicks { get; set; }
        public decimal ClickThroughRate { get; set; }
        public decimal BookmarkRate { get; set; }
        public double AverageVisitDurationSeconds { get; set; }
        public Dictionary<string, object>? UserDemographics { get; set; }
        public Dictionary<string, object>? TrafficSources { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class BusinessAnalyticsCreateDto
    {
        public Guid BusinessId { get; set; }
        public int? InitialViews { get; set; }
        public int? InitialBookmarks { get; set; }
        public int? InitialClicks { get; set; }
    }

    public class BusinessAnalyticsUpdateDto
    {
        public int? ViewsToAdd { get; set; }
        public int? BookmarksToAdd { get; set; }
        public int? ClicksToAdd { get; set; }
        public double? VisitDurationSeconds { get; set; }
        public Dictionary<string, object>? UserDemographics { get; set; }
        public Dictionary<string, object>? TrafficSources { get; set; }
    }

    public class AnalyticsSummaryDto
    {
        public int TotalBusinesses { get; set; }
        public int TotalViews { get; set; }
        public int TotalBookmarks { get; set; }
        public int TotalClicks { get; set; }
        public List<CategoryAnalyticsDto> TopCategories { get; set; } = new List<CategoryAnalyticsDto>();
        public List<BusinessAnalyticsDto> TopBusinesses { get; set; } = new List<BusinessAnalyticsDto>();
    }

    public class CategoryAnalyticsDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int TotalViews { get; set; }
        public int BusinessCount { get; set; }
    }
}
