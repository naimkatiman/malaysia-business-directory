using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MalaysiaBusinessDirectory.Api.Data;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Models;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            return tags.Select(MapToDto);
        }

        public async Task<TagDto?> GetTagByIdAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return null;

            return MapToDto(tag);
        }

        public async Task<TagDto> CreateTagAsync(TagCreateDto tagDto)
        {
            var tag = new Tag
            {
                Name = tagDto.Name,
                Description = tagDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task<TagDto?> UpdateTagAsync(Guid id, TagUpdateDto tagDto)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(tagDto.Name))
                tag.Name = tagDto.Name;
            
            if (tagDto.Description != null)
                tag.Description = tagDto.Description;

            tag.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(tag);
        }

        public async Task<bool> DeleteTagAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return false;

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to map Tag entity to TagDto
        private TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            };
        }
    }
}
