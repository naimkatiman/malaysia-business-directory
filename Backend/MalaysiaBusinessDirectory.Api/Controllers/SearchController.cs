using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public SearchController(
            IBusinessService businessService,
            ICategoryService categoryService,
            ITagService tagService)
        {
            _businessService = businessService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<SearchResultsDto>> Search([FromQuery] SearchQueryDto searchQuery)
        {
            var results = new SearchResultsDto
            {
                Businesses = new List<BusinessDto>(),
                Categories = new List<CategoryDto>(),
                Tags = new List<TagDto>()
            };

            if (string.IsNullOrWhiteSpace(searchQuery.Query) && 
                !searchQuery.CategoryId.HasValue && 
                !searchQuery.Latitude.HasValue && 
                !searchQuery.Longitude.HasValue)
            {
                return Ok(results);
            }

            // Convert to business search DTO
            var businessSearchDto = new BusinessSearchDto
            {
                Query = searchQuery.Query,
                CategoryId = searchQuery.CategoryId,
                Latitude = searchQuery.Latitude,
                Longitude = searchQuery.Longitude,
                Radius = searchQuery.Radius,
                Page = searchQuery.Page,
                PageSize = searchQuery.PageSize
            };

            // Search for businesses
            results.Businesses = (await _businessService.SearchBusinessesAsync(businessSearchDto)).ToList();

            // If there's a text query, also search for matching categories and tags
            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync();
                results.Categories = allCategories
                    .Where(c => c.Name.Contains(searchQuery.Query, StringComparison.OrdinalIgnoreCase) ||
                                (c.Description != null && c.Description.Contains(searchQuery.Query, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                var allTags = await _tagService.GetAllTagsAsync();
                results.Tags = allTags
                    .Where(t => t.Name.Contains(searchQuery.Query, StringComparison.OrdinalIgnoreCase) ||
                                (t.Description != null && t.Description.Contains(searchQuery.Query, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return Ok(results);
        }
    }

    public class SearchQueryDto
    {
        public string? Query { get; set; }
        public Guid? CategoryId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Radius { get; set; } = 10000; // Default 10km in meters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SearchResultsDto
    {
        public List<BusinessDto> Businesses { get; set; } = new List<BusinessDto>();
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
