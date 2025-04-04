using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(Guid id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag(TagCreateDto tagDto)
        {
            var tag = await _tagService.CreateTagAsync(tagDto);
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TagDto>> UpdateTag(Guid id, TagUpdateDto tagDto)
        {
            var tag = await _tagService.UpdateTagAsync(id, tagDto);
            if (tag == null)
                return NotFound();

            return Ok(tag);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
