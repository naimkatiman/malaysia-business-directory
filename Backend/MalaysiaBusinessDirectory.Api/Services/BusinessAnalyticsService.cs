using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MalaysiaBusinessDirectory.Api.Data;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Models;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public class BusinessAnalyticsService : IBusinessAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public BusinessAnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessAnalyticsDto> GetAnalyticsByIdAsync(Guid id)
        {
            var analytics = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (analytics == null)
                return null;

            return MapToDto(analytics);
        }

        public async Task<BusinessAnalyticsDto> GetAnalyticsByBusinessIdAsync(Guid businessId)
        {
            var analytics = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .FirstOrDefaultAsync(a => a.BusinessId == businessId);

            if (analytics == null)
                return null;

            return MapToDto(analytics);
        }

        public async Task<IEnumerable<BusinessAnalyticsDto>> GetTopBusinessesByViewsAsync(int limit = 10)
        {
            var topBusinesses = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .OrderByDescending(a => a.TotalViews)
                .Take(limit)
                .ToListAsync();

            return topBusinesses.Select(MapToDto);
        }

        public async Task<BusinessAnalyticsDto> CreateAnalyticsAsync(BusinessAnalyticsCreateDto createDto)
        {
            // Check if business exists
            var business = await _context.Businesses.FindAsync(createDto.BusinessId);
            if (business == null)
                throw new KeyNotFoundException($"Business with ID {createDto.BusinessId} not found");

            // Check if analytics already exist for this business
            var existingAnalytics = await _context.BusinessAnalytics
                .FirstOrDefaultAsync(a => a.BusinessId == createDto.BusinessId);

            if (existingAnalytics != null)
                throw new InvalidOperationException($"Analytics already exist for business with ID {createDto.BusinessId}");

            var analytics = new BusinessAnalytics
            {
                Id = Guid.NewGuid(),
                BusinessId = createDto.BusinessId,
                TotalViews = createDto.InitialViews ?? 0,
                WeeklyViews = createDto.InitialViews ?? 0,
                MonthlyViews = createDto.InitialViews ?? 0,
                TotalBookmarks = createDto.InitialBookmarks ?? 0,
                TotalClicks = createDto.InitialClicks ?? 0,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _context.BusinessAnalytics.Add(analytics);
            await _context.SaveChangesAsync();

            analytics.Business = business;
            return MapToDto(analytics);
        }

        public async Task<BusinessAnalyticsDto> UpdateAnalyticsAsync(Guid businessId, BusinessAnalyticsUpdateDto updateDto)
        {
            var analytics = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .FirstOrDefaultAsync(a => a.BusinessId == businessId);

            if (analytics == null)
                return null;

            // Update view counts if provided
            if (updateDto.ViewsToAdd.HasValue && updateDto.ViewsToAdd.Value > 0)
            {
                analytics.TotalViews += updateDto.ViewsToAdd.Value;
                analytics.WeeklyViews += updateDto.ViewsToAdd.Value;
                analytics.MonthlyViews += updateDto.ViewsToAdd.Value;
            }

            // Update bookmark count if provided
            if (updateDto.BookmarksToAdd.HasValue && updateDto.BookmarksToAdd.Value > 0)
            {
                analytics.TotalBookmarks += updateDto.BookmarksToAdd.Value;
            }

            // Update click count if provided
            if (updateDto.ClicksToAdd.HasValue && updateDto.ClicksToAdd.Value > 0)
            {
                analytics.TotalClicks += updateDto.ClicksToAdd.Value;
            }

            // Update average visit duration if provided
            if (updateDto.VisitDurationSeconds.HasValue && updateDto.VisitDurationSeconds.Value > 0)
            {
                // Simple running average calculation
                analytics.AverageVisitDurationSeconds = 
                    (analytics.AverageVisitDurationSeconds * analytics.TotalViews + updateDto.VisitDurationSeconds.Value) / 
                    (analytics.TotalViews + 1);
            }

            // Calculate rates
            if (analytics.TotalViews > 0)
            {
                analytics.ClickThroughRate = (decimal)analytics.TotalClicks / analytics.TotalViews * 100;
                analytics.BookmarkRate = (decimal)analytics.TotalBookmarks / analytics.TotalViews * 100;
            }

            // Update demographics if provided
            if (updateDto.UserDemographics != null)
            {
                analytics.UserDemographics = JsonSerializer.Serialize(updateDto.UserDemographics);
            }

            // Update traffic sources if provided
            if (updateDto.TrafficSources != null)
            {
                analytics.TrafficSources = JsonSerializer.Serialize(updateDto.TrafficSources);
            }

            analytics.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(analytics);
        }

        public async Task<bool> DeleteAnalyticsAsync(Guid id)
        {
            var analytics = await _context.BusinessAnalytics.FindAsync(id);
            if (analytics == null)
                return false;

            _context.BusinessAnalytics.Remove(analytics);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RecordBusinessViewAsync(Guid businessId, string? userIp = null, string? userAgent = null)
        {
            var analytics = await _context.BusinessAnalytics
                .FirstOrDefaultAsync(a => a.BusinessId == businessId);

            if (analytics == null)
            {
                // Auto-create analytics if they don't exist
                var createDto = new BusinessAnalyticsCreateDto
                {
                    BusinessId = businessId,
                    InitialViews = 1
                };
                await CreateAnalyticsAsync(createDto);
                return true;
            }

            var updateDto = new BusinessAnalyticsUpdateDto
            {
                ViewsToAdd = 1
            };

            // If we have user agent info, we could update demographics
            if (!string.IsNullOrEmpty(userAgent))
            {
                // Simplified example - in a real app you would use a proper user agent parser
                var demographics = analytics.UserDemographics != null ? 
                    JsonSerializer.Deserialize<Dictionary<string, object>>(analytics.UserDemographics) : 
                    new Dictionary<string, object>();

                // Update device counts (simplified example)
                bool isMobile = userAgent.Contains("Mobile") || userAgent.Contains("Android");
                string deviceType = isMobile ? "Mobile" : "Desktop";
                
                if (demographics.TryGetValue(deviceType, out var count))
                {
                    int currentCount = Convert.ToInt32(count);
                    demographics[deviceType] = currentCount + 1;
                }
                else
                {
                    demographics[deviceType] = 1;
                }

                updateDto.UserDemographics = demographics;
            }

            await UpdateAnalyticsAsync(businessId, updateDto);
            return true;
        }

        public async Task<bool> RecordBusinessClickAsync(Guid businessId, string clickType, string? userIp = null)
        {
            var analytics = await _context.BusinessAnalytics
                .FirstOrDefaultAsync(a => a.BusinessId == businessId);

            if (analytics == null)
                return false;

            var updateDto = new BusinessAnalyticsUpdateDto
            {
                ClicksToAdd = 1
            };

            // Track click types in traffic sources
            if (!string.IsNullOrEmpty(clickType))
            {
                var trafficSources = analytics.TrafficSources != null ? 
                    JsonSerializer.Deserialize<Dictionary<string, object>>(analytics.TrafficSources) : 
                    new Dictionary<string, object>();

                if (trafficSources.TryGetValue(clickType, out var count))
                {
                    int currentCount = Convert.ToInt32(count);
                    trafficSources[clickType] = currentCount + 1;
                }
                else
                {
                    trafficSources[clickType] = 1;
                }

                updateDto.TrafficSources = trafficSources;
            }

            await UpdateAnalyticsAsync(businessId, updateDto);
            return true;
        }

        public async Task<bool> RecordBusinessBookmarkAsync(Guid businessId, string? userId = null)
        {
            var analytics = await _context.BusinessAnalytics
                .FirstOrDefaultAsync(a => a.BusinessId == businessId);

            if (analytics == null)
                return false;

            var updateDto = new BusinessAnalyticsUpdateDto
            {
                BookmarksToAdd = 1
            };

            await UpdateAnalyticsAsync(businessId, updateDto);
            return true;
        }

        public async Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync()
        {
            var summary = new AnalyticsSummaryDto
            {
                TotalBusinesses = await _context.Businesses.CountAsync(),
                TotalViews = await _context.BusinessAnalytics.SumAsync(a => a.TotalViews),
                TotalBookmarks = await _context.BusinessAnalytics.SumAsync(a => a.TotalBookmarks),
                TotalClicks = await _context.BusinessAnalytics.SumAsync(a => a.TotalClicks)
            };

            // Get top businesses
            var topBusinesses = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .OrderByDescending(a => a.TotalViews)
                .Take(5)
                .ToListAsync();

            summary.TopBusinesses = topBusinesses.Select(MapToDto).ToList();

            // Get top categories
            var categoryAnalytics = await _context.Businesses
                .GroupJoin(
                    _context.BusinessAnalytics,
                    b => b.Id,
                    a => a.BusinessId,
                    (business, analytics) => new { Business = business, Analytics = analytics.DefaultIfEmpty() })
                .GroupBy(x => new { x.Business.CategoryId, CategoryName = x.Business.Category.Name })
                .Select(g => new CategoryAnalyticsDto
                {
                    CategoryId = g.Key.CategoryId ?? Guid.Empty,
                    CategoryName = g.Key.CategoryName ?? "Uncategorized",
                    BusinessCount = g.Count(),
                    TotalViews = g.Sum(x => x.Analytics.Sum(a => a != null ? a.TotalViews : 0))
                })
                .OrderByDescending(c => c.TotalViews)
                .Take(5)
                .ToListAsync();

            summary.TopCategories = categoryAnalytics;

            return summary;
        }

        // Helper method to map entity to DTO
        private BusinessAnalyticsDto MapToDto(BusinessAnalytics analytics)
        {
            var dto = new BusinessAnalyticsDto
            {
                Id = analytics.Id,
                BusinessId = analytics.BusinessId,
                BusinessName = analytics.Business?.Name ?? string.Empty,
                TotalViews = analytics.TotalViews,
                WeeklyViews = analytics.WeeklyViews,
                MonthlyViews = analytics.MonthlyViews,
                TotalBookmarks = analytics.TotalBookmarks,
                TotalClicks = analytics.TotalClicks,
                ClickThroughRate = analytics.ClickThroughRate,
                BookmarkRate = analytics.BookmarkRate,
                AverageVisitDurationSeconds = analytics.AverageVisitDurationSeconds,
                LastUpdated = analytics.LastUpdated
            };

            // Deserialize JSON strings if they exist
            if (!string.IsNullOrEmpty(analytics.UserDemographics))
            {
                dto.UserDemographics = JsonSerializer.Deserialize<Dictionary<string, object>>(analytics.UserDemographics);
            }

            if (!string.IsNullOrEmpty(analytics.TrafficSources))
            {
                dto.TrafficSources = JsonSerializer.Deserialize<Dictionary<string, object>>(analytics.TrafficSources);
            }

            return dto;
        }
    }
}
