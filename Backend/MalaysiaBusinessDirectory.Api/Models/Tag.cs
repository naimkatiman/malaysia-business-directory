using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MalaysiaBusinessDirectory.Api.Models
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for the join table
        public ICollection<BusinessTag> BusinessTags { get; set; } = new List<BusinessTag>();
    }
}
