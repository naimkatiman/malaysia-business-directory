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
}
