using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MalaysiaBusinessDirectory.Api.DTOs;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public interface IBusinessAnalyticsService
    {
        Task<BusinessAnalyticsDto> GetAnalyticsByIdAsync(Guid id);
        Task<BusinessAnalyticsDto> GetAnalyticsByBusinessIdAsync(Guid businessId);
        Task<IEnumerable<BusinessAnalyticsDto>> GetTopBusinessesByViewsAsync(int limit = 10);
        Task<BusinessAnalyticsDto> CreateAnalyticsAsync(BusinessAnalyticsCreateDto createDto);
        Task<BusinessAnalyticsDto> UpdateAnalyticsAsync(Guid businessId, BusinessAnalyticsUpdateDto updateDto);
        Task<bool> DeleteAnalyticsAsync(Guid id);
        Task<bool> RecordBusinessViewAsync(Guid businessId, string? userIp = null, string? userAgent = null);
        Task<bool> RecordBusinessClickAsync(Guid businessId, string clickType, string? userIp = null);
        Task<bool> RecordBusinessBookmarkAsync(Guid businessId, string? userId = null);
        Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync();
    }
}
