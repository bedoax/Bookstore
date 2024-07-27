using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(BookstoreDbContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Reviews
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult> GetReviews()
        {
            _logger.LogInformation("Getting all reviews.");
            var reviews = await _context.Reviews
                            .Include(r => r.Book)
                            .Select(r => new
                            {
                                r.Id,
                                r.Rating,
                                r.Comment,
                                r.ReviewDate,
                                r.Likes,
                                r.Dislikes,
                                r.Response,
                                r.ResponseDate,
                                Book = new
                                {
                                    r.Book.Title,
                                }
                            })
                            .ToListAsync();

            if (reviews == null || !reviews.Any())
            {
                _logger.LogWarning("No reviews found.");
                return NotFound();
            }

            return Ok(reviews);
        }

        // GET: api/Reviews/5
        [HttpGet("{rateNumber}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReview(int rateNumber)
        {
            _logger.LogInformation("Getting reviews with rating: {RateNumber}", rateNumber);
            var reviews = await _context.Reviews.Where(x => x.Rating == rateNumber).ToListAsync();

            if (!reviews.Any())
            {
                _logger.LogWarning("No reviews found with rating: {RateNumber}", rateNumber);
                return NotFound();
            }

            return reviews;
        }

        [HttpGet("GetReviewByHighestLikes")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewByHighestLikes()
        {
            _logger.LogInformation("Getting reviews ordered by highest likes.");
            var topLikes = await _context.Reviews
                .OrderByDescending(x => x.Likes)
                .ThenBy(x => x.Dislikes)
                .ThenByDescending(x => x.Rating)
                .ToListAsync();

            if (!topLikes.Any())
            {
                _logger.LogWarning("No reviews found sorted by highest likes.");
                return NotFound();
            }

            return topLikes;
        }

        [HttpGet("GetReviewByHighestRatings")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewByHighestRatings()
        {
            _logger.LogInformation("Getting reviews ordered by highest ratings.");
            var topRate = await _context.Reviews
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Likes)
                .ThenBy(x => x.Dislikes)
                .ToListAsync();

            if (!topRate.Any())
            {
                _logger.LogWarning("No reviews found sorted by highest ratings.");
                return NotFound();
            }

            return topRate;
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AllowCustomer")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Id)
            {
                _logger.LogWarning("Mismatch between route ID: {RouteId} and review ID: {ReviewId}.", id, review.Id);
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Review with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    _logger.LogWarning("Review with ID: {Id} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception while updating review with ID: {Id}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reviews
        [HttpPost]
        [Authorize(Policy = "AllowCustomer")]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            if (_context.Books.Any(x => x.Id == review.BookID) && _context.Customers.Any(x => x.Id == review.CustomerID))
            {
                _logger.LogInformation("Adding new review for book ID: {BookId} and customer ID: {CustomerId}.", review.BookID, review.CustomerID);
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetReview", new { id = review.Id }, review);
            }
            _logger.LogWarning("Failed to add review: Book ID {BookId} or Customer ID {CustomerId} does not exist.", review.BookID, review.CustomerID);
            return BadRequest();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            _logger.LogInformation("Deleting review with ID: {Id}", id);
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                _logger.LogWarning("Review with ID: {Id} not found.", id);
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Review with ID: {Id} deleted successfully.", id);
            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
