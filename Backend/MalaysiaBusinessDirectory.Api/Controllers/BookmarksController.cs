using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/bookmarks")]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly IBusinessService _businessService;
        private readonly IUserService _userService;

        public BookmarksController(
            IBookmarkService bookmarkService,
            IBusinessService businessService,
            IUserService userService)
        {
            _bookmarkService = bookmarkService;
            _businessService = businessService;
            _userService = userService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookmarkDto>>> GetUserBookmarks(Guid userId)
        {
            // Validate user exists
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var bookmarks = await _bookmarkService.GetBookmarksByUserIdAsync(userId);
            return Ok(bookmarks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookmarkDto>> GetBookmark(Guid id)
        {
            var bookmark = await _bookmarkService.GetBookmarkByIdAsync(id);
            if (bookmark == null)
                return NotFound();

            return Ok(bookmark);
        }

        [HttpPost]
        public async Task<ActionResult<BookmarkDto>> CreateBookmark(BookmarkCreateDto bookmarkDto)
        {
            // Validate user exists
            var user = await _userService.GetUserByIdAsync(bookmarkDto.UserId);
            if (user == null)
                return BadRequest("User not found");

            // Validate business exists
            var business = await _businessService.GetBusinessByIdAsync(bookmarkDto.BusinessId);
            if (business == null)
                return BadRequest("Business not found");

            // Check if bookmark already exists
            var existingBookmark = await _bookmarkService.GetBookmarkAsync(bookmarkDto.UserId, bookmarkDto.BusinessId);
            if (existingBookmark != null)
                return BadRequest("Bookmark already exists");

            var bookmark = await _bookmarkService.CreateBookmarkAsync(bookmarkDto);
            return CreatedAtAction(nameof(GetBookmark), new { id = bookmark.Id }, bookmark);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBookmark(Guid id)
        {
            var result = await _bookmarkService.DeleteBookmarkAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("user/{userId}/business/{businessId}")]
        public async Task<ActionResult> DeleteUserBusinessBookmark(Guid userId, Guid businessId)
        {
            var bookmark = await _bookmarkService.GetBookmarkAsync(userId, businessId);
            if (bookmark == null)
                return NotFound();

            var result = await _bookmarkService.DeleteBookmarkAsync(bookmark.Id);
            if (!result)
                return BadRequest("Failed to delete bookmark");

            return NoContent();
        }

        [HttpGet("business/{businessId}")]
        public async Task<ActionResult<BusinessBookmarkStatsDto>> GetBusinessBookmarkStatistics(Guid businessId)
        {
            // Validate business exists
            var business = await _businessService.GetBusinessByIdAsync(businessId);
            if (business == null)
                return NotFound("Business not found");

            var stats = await _bookmarkService.GetBusinessBookmarkStatsAsync(businessId);
            return Ok(stats);
        }
        
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckBookmarkExists([FromQuery] Guid userId, [FromQuery] Guid businessId)
        {
            var bookmark = await _bookmarkService.GetBookmarkAsync(userId, businessId);
            return Ok(bookmark != null);
        }
    }

    public class BookmarkDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string? BusinessImageUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookmarkCreateDto
    {
        public Guid UserId { get; set; }
        public Guid BusinessId { get; set; }
        public string? Notes { get; set; }
    }

    public class BusinessBookmarkStatsDto
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; }
        public int BookmarkCount { get; set; }
    }
}
