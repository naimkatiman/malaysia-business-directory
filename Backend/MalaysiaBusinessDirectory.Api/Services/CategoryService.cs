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
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            return MapToDto(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Icon = categoryDto.Icon,
                Image = categoryDto.Image,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(Guid id, CategoryUpdateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(categoryDto.Name))
                category.Name = categoryDto.Name;
            
            if (categoryDto.Description != null)
                category.Description = categoryDto.Description;
            
            if (categoryDto.Icon != null)
                category.Icon = categoryDto.Icon;
            
            if (categoryDto.Image != null)
                category.Image = categoryDto.Image;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper method to map Category entity to CategoryDto
        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                Image = category.Image,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
