using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Caching.Memory;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly IMemoryCache _cache;
        //private readonly ILogger _logger;
        public BookController(BookstoreDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            //_logger = logger;
        }



        // GET: api/Books
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var cacheKey = "books";
            if(!_cache.TryGetValue(cacheKey,out List<Book> books))
            {
                books = await _context.Books.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                _cache.Set(cacheKey, books,cacheEntryOptions);

            }
            return Ok(books);
        }

        [HttpGet("GetAvailableBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetAvailableBooks([FromQuery] bool inStock = true)
        {
            var query = _context.Books.AsQueryable();

            if (inStock)
            {
                query = query.Where(b => b.Quantity > 0);
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }
        [HttpGet("GetBooksByPublishingYear")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByPublishingYear([FromQuery] int start, [FromQuery] int end)
        {
            if (start > end)
            {
                return BadRequest("The start year cannot be greater than the end year.");
            }

            var books = await _context.Books
                .Where(x => x.PublishedDate.Year >= start && x.PublishedDate.Year <= end)
                .ToListAsync();

            if (books == null || !books.Any())
            {
                return NotFound("No books found within the specified date range.");
            }

            return Ok(books);
        }


        [HttpGet("GetBooksByRatingsOfCustomer")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByRatings([FromQuery] int rate)
        {
            //var reviews = _context.Reviews.Where(x => x.Rating == rate).Select(x=>x.BookID).ToListAsync();
            //var books = _context.Books.FirstOrDefault(x => x.Id == reviews.Id).ToListAsync();
            var getBooksByRate = await (from b in _context.Books
                                        join r in _context.Reviews
                                        on b.Id equals r.BookID
                                        where r.Rating == rate
                                        select new { b.Title, r.Rating }).ToListAsync();
            if (!getBooksByRate.Any() || getBooksByRate == null)
            {
                return BadRequest();
            }
            return Ok(getBooksByRate);

        }

        [HttpGet("SortBooks/{sortBy}/{descending}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks(string sortBy, bool descending)
        {
            IQueryable<Book> query = _context.Books;

            switch (sortBy.ToLower())
            {
                case "price":
                    query = descending ?
                        query.OrderByDescending(b => b.Price) :
                        query.OrderBy(b => b.Price);
                    break;

                /*                case "rating":
                    // Ensure your Book entity or a related entity can provide the average rating
                    query = descending ?
                        query.OrderByDescending(b => b.AverageRating) :
                        query.OrderBy(b => b.AverageRating);
                    break;*/

                default:
                    return BadRequest("Invalid sort criteria.");
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }

        [HttpGet("SearchBooks/{searchBy}/{valueOfSearch}/{descending}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksBySearching(string searchBy, string valueOfSearch, bool descending)
        {
            IQueryable<Book> query = _context.Books;

            if (string.IsNullOrEmpty(valueOfSearch))
            {
                return BadRequest("Search value cannot be empty.");
            }

            switch (searchBy.ToLower())
            {
                case "author":
                    query = query.Where(b => b.Author.Name.Contains(valueOfSearch)); // Filter first
                    query = descending ?
                        query.OrderByDescending(b => b.Author.Name) :
                        query.OrderBy(b => b.Author.Name);
                    break;

                case "category":
                    query = query.Where(b => b.Category.CategoryName.Contains(valueOfSearch)); // Filter first
                    query = descending ?
                        query.OrderByDescending(b => b.Category.CategoryName) :
                        query.OrderBy(b => b.Category.CategoryName);
                    break;

                default:
                    return BadRequest("Invalid search criteria.");
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }



        [HttpGet("FullTextSearch")]
        public async Task<ActionResult<IEnumerable<Book>>>FullTextSearch(
            [FromQuery] string searchTerm,
            [FromQuery] string sortBy = "Title",
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term cannot by empty.");
            }
            IQueryable<Book> query = _context.Books
                .Where(b => b.Title.Contains(searchTerm) ||
                            b.Description.Contains(searchTerm) ||
                            b.Author.Name.Contains(searchTerm) ||
                            b.Category.CategoryName.Contains(searchTerm));
            // Sorting
            switch (sortBy.ToLower())
            {
                case "title":
                    query = descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title);
                    break;
                case "price":
                    query = descending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price);
                    break;
                case "rating":
                    query = descending ? query.OrderByDescending(b => b.Reviews.Average(r => r.Rating)) : query.OrderBy(b => b.Reviews.Average(r => r.Rating));
                    break;
                default:
                    query = query.OrderBy(b => b.Title);
                    break;
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Books = books
            };

            return Ok(result);
        }

        /// <summary>
        /// Advanced search for books with multiple filtering, sorting, and pagination options.
        /// </summary>
        /// <param name="author">Filter by author's name (partial match).</param>
        /// <param name="category">Filter by category name (partial match).</param>
        /// <param name="minPrice">Filter by minimum price.</param>
        /// <param name="maxPrice">Filter by maximum price.</param>
        /// <param name="minRating">Filter by minimum rating.</param>
        /// <param name="maxRating">Filter by maximum rating.</param>
        /// <param name="startYear">Filter by start year.</param>
        /// <param name="endYear">Filter by end year.</param>
        /// <param name="inStock">Give To use option for show item in stock or not.</param>
        /// <param name="sortBy">Field to sort by (e.g., "Title", "Price", "Rating").</param>
        /// <param name="descending">Sort in descending order if true; otherwise, ascending.</param>
        /// <param name="page">Page number for pagination (starting at 1).</param>
        /// <param name="pageSize">Number of items per page for pagination.</param>
        /// <returns>Paginated list of books matching the search criteria.</returns>
        [HttpGet("AdvancedSearch")]
        public async Task<ActionResult<IEnumerable<Book>>> AdvancedSearch(
            [FromQuery] string author = null,
            [FromQuery] string category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] double? minRating = null,
            [FromQuery] double? maxRating = null,
            [FromQuery] string searchText = null,
            [FromQuery] string sortBy = "Title",
            [FromQuery] bool descending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            IQueryable<Book> query = _context.Books;

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Name.Contains(author));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category.CategoryName.Contains(category));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(b => b.Reviews.Any(r => r.Rating >= minRating.Value));
            }

            if (maxRating.HasValue)
            {
                query = query.Where(b => b.Reviews.Any(r => r.Rating <= maxRating.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower(); // Normalize the search text
                query = query.Where(b => b.Title.ToLower().Contains(searchText) ||
                                         b.Description.ToLower().Contains(searchText) ||
                                         b.Author.Name.ToLower().Contains(searchText));
            }

            // Sorting
            switch (sortBy.ToLower())
            {
                case "title":
                    query = descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title);
                    break;
                case "price":
                    query = descending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price);
                    break;
                case "rating":
                    query = descending ? query.OrderByDescending(b => b.Reviews.Average(r => r.Rating)) : query.OrderBy(b => b.Reviews.Average(r => r.Rating));
                    break;
                default:
                    query = query.OrderBy(b => b.Title);
                    break;
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Books = books
            };

            return Ok(result);
        }






        [HttpGet("GetPopularBooks")]
        public async Task<ActionResult<IEnumerable<object>>> GetPopularBooks()
        {
            // Get popular books with their counts
            var popularBooks = await _context.OrderHistories
                .GroupBy(oh => oh.BookID)
                .Where(g => g.Count() >= 1)
                .Select(g => new
                {
                    BookId = g.Key,
                    NumberOfRecommended = g.Count()
                })
                .ToListAsync();

            // Extract book IDs from the popular books
            var bookIds = popularBooks.Select(pb => pb.BookId).ToList();

            // Get book titles for the popular book IDs
            var books = await _context.Books
                .Where(b => bookIds.Contains(b.Id))
                .Select(b => new
                {
                    b.Id,
                    b.Title
                })
                .ToListAsync();

            // Join the book titles with their counts
            var result = popularBooks
                .Join(books,
                      pb => pb.BookId,
                      b => b.Id,
                      (pb, b) => new
                      {
                          BookName = b.Title,
                          NumberOfRecommended = pb.NumberOfRecommended
                      })
                .ToList();

            // Return the result
            return Ok(result);
        }





        // GET: api/Books/5
        [HttpGet("{title}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<Book>> GetBook(string title)
        {
            //var book = await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.Title == title);
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Title.ToLower() == title.ToLower());

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                //_logger.LogWarning("Invalid model state for book: {@Book}", book);
                return BadRequest(ModelState); // Return validation errors
            }
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            book.Id = 0;
            if (!ModelState.IsValid)
            {
                //_logger.LogWarning("Invalid model state for book: {@Book}", book);
                return BadRequest(ModelState); // Return validation errors
            }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
