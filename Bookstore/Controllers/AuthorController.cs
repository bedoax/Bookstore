using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(BookstoreDbContext context, ILogger<AuthorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            _logger.LogInformation("Retrieving all authors");
            var authors = await _context.Authors.ToListAsync();
            _logger.LogInformation("Successfully retrieved authors");
            return authors;
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            _logger.LogInformation("Retrieving author with ID {Id}", id);
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Author with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved author with ID {Id}", id);
            return author;
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                _logger.LogWarning("Author ID mismatch: {Id} != {AuthorId}", id, author.Id);
                return BadRequest();
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated author with ID {Id}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    _logger.LogWarning("Author with ID {Id} does not exist", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception occurred while updating author with ID {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            _logger.LogInformation("Adding a new author");
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added author with ID {Id}", author.Id);

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            _logger.LogInformation("Deleting author with ID {Id}", id);
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                _logger.LogWarning("Author with ID {Id} not found", id);
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted author with ID {Id}", id);

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            _logger.LogInformation("Checking if author with ID {Id} exists", id);
            var exists = _context.Authors.Any(e => e.Id == id);
            _logger.LogInformation("Author with ID {Id} exists: {Exists}", id, exists);
            return exists;
        }
    }
}
