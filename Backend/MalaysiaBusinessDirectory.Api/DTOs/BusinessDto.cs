using System;
using System.Collections.Generic;

namespace MalaysiaBusinessDirectory.Api.DTOs
{
    public class BusinessDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string[]? Images { get; set; }
        public string? OpeningHours { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TagDto>? Tags { get; set; }
    }

    public class BusinessCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string[]? Images { get; set; }
        public string? OpeningHours { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? TagIds { get; set; }
    }

    public class BusinessUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string[]? Images { get; set; }
        public string? OpeningHours { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? TagIds { get; set; }
    }

    public class BusinessSearchDto
    {
        public string? Query { get; set; }
        public Guid? CategoryId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Radius { get; set; } = 10000; // Default 10km in meters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
