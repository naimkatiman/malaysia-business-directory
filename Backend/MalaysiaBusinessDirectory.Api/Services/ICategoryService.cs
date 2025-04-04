using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MalaysiaBusinessDirectory.Api.DTOs;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto?> UpdateCategoryAsync(Guid id, CategoryUpdateDto categoryDto);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
