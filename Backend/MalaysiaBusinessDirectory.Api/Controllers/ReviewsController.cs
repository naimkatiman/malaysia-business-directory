using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MalaysiaBusinessDirectory.Api.DTOs;
using MalaysiaBusinessDirectory.Api.Services;

namespace MalaysiaBusinessDirectory.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IBusinessService _businessService;

        public ReviewsController(IReviewService reviewService, IBusinessService businessService)
        {
            _reviewService = reviewService;
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews([FromQuery] Guid? businessId)
        {
            if (businessId.HasValue)
            {
                var reviews = await _reviewService.GetReviewsByBusinessIdAsync(businessId.Value);
                return Ok(reviews);
            }
            
            var allReviews = await _reviewService.GetAllReviewsAsync();
            return Ok(allReviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(ReviewCreateDto reviewDto)
        {
            // Check if business exists
            var business = await _businessService.GetBusinessByIdAsync(reviewDto.BusinessId);
            if (business == null)
                return BadRequest("Business not found");

            var review = await _reviewService.CreateReviewAsync(reviewDto);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewDto>> UpdateReview(Guid id, ReviewUpdateDto reviewDto)
        {
            var review = await _reviewService.UpdateReviewAsync(id, reviewDto);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public List<string> PhotoUrls { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int HelpfulCount { get; set; }
    }
    
    public class ReviewCreateDto
    {
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } // 1-5
        public List<string>? PhotoUrls { get; set; }
    }

    public class ReviewUpdateDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int? Rating { get; set; }
        public List<string>? PhotoUrls { get; set; }
    }
}
