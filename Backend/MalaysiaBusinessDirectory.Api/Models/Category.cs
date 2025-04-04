using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MalaysiaBusinessDirectory.Api.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Icon { get; set; }

        [MaxLength(255)]
        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for businesses in this category
        public ICollection<Business> Businesses { get; set; } = new List<Business>();
    }
}
