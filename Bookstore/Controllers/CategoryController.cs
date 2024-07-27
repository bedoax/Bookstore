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
    public class CategoryController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(BookstoreDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            _logger.LogInformation("Fetching all categories.");
            var categories = await _context.Categories.ToListAsync();
            _logger.LogInformation("Retrieved {Count} categories.", categories.Count);
            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            _logger.LogInformation("Fetching category with ID {Id}.", id);
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved category with ID {Id}.", id);
            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                _logger.LogWarning("Category ID in the URL ({UrlId}) does not match the category ID in the body ({BodyId}).", id, category.Id);
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated category with ID {Id}.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    _logger.LogWarning("Category with ID {Id} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception occurred while updating category with ID {Id}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new category with ID {Id}.", category.Id);

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("Deleting category with ID {Id}.", id);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted category with ID {Id}.", id);

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            bool exists = _context.Categories.Any(e => e.Id == id);
            _logger.LogInformation("Category with ID {Id} exists: {Exists}.", id, exists);
            return exists;
        }
    }
}
