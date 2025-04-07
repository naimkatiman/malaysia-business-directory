using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IBusinessAnalyticsService _analyticsService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IBusinessAnalyticsService analyticsService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<AnalyticsSummaryDto>> GetAnalyticsSummary()
        {
            try
            {
                var summary = await _analyticsService.GetAnalyticsSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics summary");
                return StatusCode(500, "An error occurred while retrieving analytics summary");
            }
        }

        [HttpGet("business/{businessId}")]
        public async Task<ActionResult<BusinessAnalyticsDto>> GetBusinessAnalytics(Guid businessId)
        {
            try
            {
                var analytics = await _analyticsService.GetAnalyticsByBusinessIdAsync(businessId);
                if (analytics == null)
                    return NotFound($"No analytics found for business with ID {businessId}");

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics for business {BusinessId}", businessId);
                return StatusCode(500, $"An error occurred while retrieving analytics for business {businessId}");
            }
        }

        [HttpGet("top-businesses")]
        public async Task<ActionResult<IEnumerable<BusinessAnalyticsDto>>> GetTopBusinesses([FromQuery] int limit = 10)
        {
            try
            {
                var topBusinesses = await _analyticsService.GetTopBusinessesByViewsAsync(limit);
                return Ok(topBusinesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top businesses");
                return StatusCode(500, "An error occurred while retrieving top businesses");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BusinessAnalyticsDto>> CreateAnalytics(BusinessAnalyticsCreateDto createDto)
        {
            try
            {
                var analytics = await _analyticsService.CreateAnalyticsAsync(createDto);
                return CreatedAtAction(nameof(GetBusinessAnalytics), new { businessId = analytics.BusinessId }, analytics);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating analytics for business {BusinessId}", createDto.BusinessId);
                return StatusCode(500, $"An error occurred while creating analytics for business {createDto.BusinessId}");
            }
        }

        [HttpPut("{businessId}")]
        public async Task<ActionResult<BusinessAnalyticsDto>> UpdateAnalytics(Guid businessId, BusinessAnalyticsUpdateDto updateDto)
        {
            try
            {
                var analytics = await _analyticsService.UpdateAnalyticsAsync(businessId, updateDto);
                if (analytics == null)
                    return NotFound($"No analytics found for business with ID {businessId}");

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating analytics for business {BusinessId}", businessId);
                return StatusCode(500, $"An error occurred while updating analytics for business {businessId}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnalytics(Guid id)
        {
            try
            {
                var result = await _analyticsService.DeleteAnalyticsAsync(id);
                if (!result)
                    return NotFound($"No analytics found with ID {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting analytics with ID {Id}", id);
                return StatusCode(500, $"An error occurred while deleting analytics with ID {id}");
            }
        }

        [HttpPost("record-view/{businessId}")]
        public async Task<ActionResult> RecordBusinessView(Guid businessId)
        {
            try
            {
                // Extract user IP and agent from the request
                string? userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                string? userAgent = Request.Headers.UserAgent;

                var result = await _analyticsService.RecordBusinessViewAsync(businessId, userIp, userAgent);
                if (!result)
                    return NotFound($"No business found with ID {businessId}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording view for business {BusinessId}", businessId);
                return StatusCode(500, $"An error occurred while recording view for business {businessId}");
            }
        }

        [HttpPost("record-click/{businessId}")]
        public async Task<ActionResult> RecordBusinessClick(Guid businessId, [FromQuery] string clickType)
        {
            try
            {
                string? userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

                var result = await _analyticsService.RecordBusinessClickAsync(businessId, clickType, userIp);
                if (!result)
                    return NotFound($"No business found with ID {businessId}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording click for business {BusinessId}", businessId);
                return StatusCode(500, $"An error occurred while recording click for business {businessId}");
            }
        }

        [HttpPost("record-bookmark/{businessId}")]
        public async Task<ActionResult> RecordBusinessBookmark(Guid businessId, [FromQuery] string? userId = null)
        {
            try
            {
                var result = await _analyticsService.RecordBusinessBookmarkAsync(businessId, userId);
                if (!result)
                    return NotFound($"No business found with ID {businessId}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording bookmark for business {BusinessId}", businessId);
                return StatusCode(500, $"An error occurred while recording bookmark for business {businessId}");
            }
        }
    }
}
