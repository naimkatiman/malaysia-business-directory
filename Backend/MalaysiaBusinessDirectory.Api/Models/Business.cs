using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace MalaysiaBusinessDirectory.Api.Models
{
    public class Business
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(255)]
        public string? City { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Website { get; set; }

        // Store images as comma-separated strings or consider a separate table for images
        [MaxLength(2000)]
        public string? Images { get; set; }

        // Store opening hours as JSON string or consider a separate table
        [MaxLength(1000)]
        public string? OpeningHours { get; set; }

        // Store location data as NetTopologySuite Point
        public Point? Location { get; set; }

        // Foreign key for Category
        public Guid? CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for business tags
        public ICollection<BusinessTag> BusinessTags { get; set; } = new List<BusinessTag>();
    }
}
