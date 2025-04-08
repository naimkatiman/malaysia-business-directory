using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MalaysiaBusinessDirectory.Api.Data;
using MalaysiaBusinessDirectory.Api.Models;

namespace MalaysiaBusinessDirectory.Api.Utilities
{
    /// <summary>
    /// Utility class for exporting business directory data in various formats
    /// </summary>
    public class DataExportUtility
    {
        private readonly ApplicationDbContext _context;

        public DataExportUtility(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Export businesses to CSV format
        /// </summary>
        /// <param name="filePath">Path where CSV file will be saved</param>
        /// <returns>Number of businesses exported</returns>
        public async Task<int> ExportBusinessesToCsvAsync(string filePath)
        {
            var businesses = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                .ThenInclude(bt => bt.Tag)
                .ToListAsync();

            if (!businesses.Any())
                return 0;

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("Id,Name,Description,Address,City,PostalCode,Phone,Email,Website,CategoryName,Tags,CreatedAt");

                // Write data
                foreach (var business in businesses)
                {
                    var tags = string.Join("|", business.BusinessTags.Select(bt => bt.Tag.Name));
                    var line = $"\"{business.Id}\",\"{EscapeCsvField(business.Name)}\",\"{EscapeCsvField(business.Description)}\",\"{EscapeCsvField(business.Address)}\",\"{EscapeCsvField(business.City)}\",\"{EscapeCsvField(business.PostalCode)}\",\"{EscapeCsvField(business.Phone)}\",\"{EscapeCsvField(business.Email)}\",\"{EscapeCsvField(business.Website)}\",\"{EscapeCsvField(business.Category?.Name)}\",\"{EscapeCsvField(tags)}\",\"{business.CreatedAt:yyyy-MM-dd HH:mm:ss}\"";
                    writer.WriteLine(line);
                }
            }

            return businesses.Count;
        }

        /// <summary>
        /// Export businesses to JSON format
        /// </summary>
        /// <param name="filePath">Path where JSON file will be saved</param>
        /// <returns>Number of businesses exported</returns>
        public async Task<int> ExportBusinessesToJsonAsync(string filePath)
        {
            var businesses = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.BusinessTags)
                .ThenInclude(bt => bt.Tag)
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Description,
                    b.Address,
                    b.City,
                    b.PostalCode,
                    b.Phone,
                    b.Email,
                    b.Website,
                    Category = b.Category != null ? b.Category.Name : null,
                    Tags = b.BusinessTags.Select(bt => bt.Tag.Name).ToList(),
                    b.CreatedAt,
                    b.UpdatedAt
                })
                .ToListAsync();

            if (!businesses.Any())
                return 0;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(businesses, options);
            await File.WriteAllTextAsync(filePath, jsonString);

            return businesses.Count;
        }

        /// <summary>
        /// Export analytics data to JSON format
        /// </summary>
        /// <param name="filePath">Path where JSON file will be saved</param>
        /// <returns>Number of analytics records exported</returns>
        public async Task<int> ExportAnalyticsToJsonAsync(string filePath)
        {
            var analytics = await _context.BusinessAnalytics
                .Include(a => a.Business)
                .Select(a => new
                {
                    a.Id,
                    a.BusinessId,
                    BusinessName = a.Business.Name,
                    a.TotalViews,
                    a.WeeklyViews,
                    a.MonthlyViews,
                    a.TotalBookmarks,
                    a.TotalClicks,
                    a.ClickThroughRate,
                    a.BookmarkRate,
                    a.AverageVisitDurationSeconds,
                    a.LastUpdated,
                    a.CreatedAt
                })
                .ToListAsync();

            if (!analytics.Any())
                return 0;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(analytics, options);
            await File.WriteAllTextAsync(filePath, jsonString);

            return analytics.Count;
        }

        /// <summary>
        /// Helper method to escape CSV field content
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;
                
            return field.Replace("\"", "\"\"");
        }
    }
}
