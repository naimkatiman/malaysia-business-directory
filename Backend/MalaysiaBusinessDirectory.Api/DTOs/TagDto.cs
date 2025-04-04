using System;

namespace MalaysiaBusinessDirectory.Api.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TagCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class TagUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
