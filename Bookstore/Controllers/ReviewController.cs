using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public ReviewController(BookstoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult> GetReviews()
        {
            //var reviews =  await _context.Reviews.ToListAsync();
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
                return NotFound();
            }

            return Ok(reviews);
        }

        // GET: api/Reviews/5
        [HttpGet("{rateNumber}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReview(int rateNumber)
        {
            var reviews = await _context.Reviews.Where(x=>x.Rating == rateNumber).ToListAsync();
            //var review = await _context.Reviews.FindAsync(id);

            if (!reviews.Any())
            {
                return NotFound();
            }

            return reviews;
        }
        [HttpGet("GetReviewByHighestLikes")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewByHighestLikes()
        {
            var topLikes = await _context.Reviews.OrderByDescending(x => x.Likes).ThenBy(x=>x.Dislikes).ThenByDescending(x=>x.Rating).ToListAsync();

            if (!topLikes.Any())
            {
                return NotFound();
            }

            return topLikes;
        }
        [HttpGet("GetReviewByHighestRatings")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewByHighestRatings()
        {
            var topRate = await _context.Reviews.OrderByDescending(x => x.Rating).ThenByDescending(x=>x.Likes).ThenBy(x=>x.Dislikes).ToListAsync();

            if (!topRate.Any())
            {
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
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
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
            // add 
            if (_context.Books.Any(x => x.Id == review.BookID) && _context.Customers.Any(x => x.Id == review.CustomerID))
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetReview", new { id = review.Id }, review);
            }
            return BadRequest();
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}
