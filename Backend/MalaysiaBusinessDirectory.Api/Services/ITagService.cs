using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MalaysiaBusinessDirectory.Api.DTOs;

namespace MalaysiaBusinessDirectory.Api.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto?> GetTagByIdAsync(Guid id);
        Task<TagDto> CreateTagAsync(TagCreateDto tagDto);
        Task<TagDto?> UpdateTagAsync(Guid id, TagUpdateDto tagDto);
        Task<bool> DeleteTagAsync(Guid id);
    }
}
