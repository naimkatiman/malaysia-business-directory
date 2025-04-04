using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MalaysiaBusinessDirectory.Api.Models
{
    public class BusinessTag
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BusinessId { get; set; }
        
        [ForeignKey("BusinessId")]
        public Business Business { get; set; } = null!;

        public Guid TagId { get; set; }
        
        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
