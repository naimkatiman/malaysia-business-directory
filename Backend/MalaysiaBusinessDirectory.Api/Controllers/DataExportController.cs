using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MalaysiaBusinessDirectory.Api.Utilities;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class DataExportController : ControllerBase
    {
        private readonly DataExportUtility _exportUtility;
        private readonly ILogger<DataExportController> _logger;
        private readonly string _exportDirectory;

        public DataExportController(
            DataExportUtility exportUtility,
            ILogger<DataExportController> logger)
        {
            _exportUtility = exportUtility;
            _logger = logger;
            
            // Create exports directory if it doesn't exist
            _exportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "exports");
            if (!Directory.Exists(_exportDirectory))
            {
                Directory.CreateDirectory(_exportDirectory);
            }
        }

        [HttpGet("businesses/csv")]
        public async Task<IActionResult> ExportBusinessesToCsv()
        {
            try
            {
                string fileName = $"businesses_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = Path.Combine(_exportDirectory, fileName);
                
                int count = await _exportUtility.ExportBusinessesToCsvAsync(filePath);
                
                if (count == 0)
                {
                    return NotFound("No businesses found to export");
                }
                
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting businesses to CSV");
                return StatusCode(500, "An error occurred while exporting businesses to CSV");
            }
        }

        [HttpGet("businesses/json")]
        public async Task<IActionResult> ExportBusinessesToJson()
        {
            try
            {
                string fileName = $"businesses_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(_exportDirectory, fileName);
                
                int count = await _exportUtility.ExportBusinessesToJsonAsync(filePath);
                
                if (count == 0)
                {
                    return NotFound("No businesses found to export");
                }
                
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting businesses to JSON");
                return StatusCode(500, "An error occurred while exporting businesses to JSON");
            }
        }

        [HttpGet("analytics/json")]
        public async Task<IActionResult> ExportAnalyticsToJson()
        {
            try
            {
                string fileName = $"analytics_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(_exportDirectory, fileName);
                
                int count = await _exportUtility.ExportAnalyticsToJsonAsync(filePath);
                
                if (count == 0)
                {
                    return NotFound("No analytics data found to export");
                }
                
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting analytics to JSON");
                return StatusCode(500, "An error occurred while exporting analytics to JSON");
            }
        }
    }
}
