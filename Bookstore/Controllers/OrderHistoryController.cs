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
    public class OrderHistoryController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<OrderHistoryController> _logger;

        public OrderHistoryController(BookstoreDbContext context, ILogger<OrderHistoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/OrderHistories
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderHistories()
        {
            _logger.LogInformation("Getting all order histories.");
            var orderHistories = await _context.OrderHistories
                .Include(o => o.Book)
                .Include(o => o.Customer)
                .Select(o => new
                {
                    OrderId = o.Id,
                    CustomerId = o.Customer.Id,
                    CustomerName = o.Customer.Name,
                    CustomerUsername = o.Customer.Username,
                    CustomerEmail = o.Customer.Email,
                    BookId = o.Book.Id,
                    BookTitle = o.Book.Title,
                    BookISBN = o.Book.ISBN,
                    BookPrice = o.Book.Price,
                    OrderDate = o.OrderDate,
                })
                .ToListAsync();

            if (!orderHistories.Any())
            {
                _logger.LogWarning("No order histories found.");
                return NoContent();
            }

            return Ok(orderHistories);
        }

        // GET: api/OrderHistories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<OrderHistory>> GetOrderHistory(int id)
        {
            _logger.LogInformation("Getting order history with ID: {Id}", id);
            var orderHistory = await _context.OrderHistories.FindAsync(id);

            if (orderHistory == null)
            {
                _logger.LogWarning("Order history with ID: {Id} not found.", id);
                return NotFound();
            }

            return orderHistory;
        }

        // PUT: api/OrderHistories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutOrderHistory(int id, OrderHistory orderHistory)
        {
            if (id != orderHistory.Id)
            {
                _logger.LogWarning("Mismatch between route ID: {RouteId} and order history ID: {OrderHistoryId}.", id, orderHistory.Id);
                return BadRequest();
            }

            _context.Entry(orderHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order history with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoryExists(id))
                {
                    _logger.LogWarning("Order history with ID: {Id} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception while updating order history with ID: {Id}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OrderHistories
        [HttpPost]
        [Authorize(Policy = "AllowCustomer")]
        public async Task<ActionResult<OrderHistory>> PostOrderHistory(OrderHistory orderHistory)
        {
            if (_context.Books.Any(x => x.Id == orderHistory.BookID) && _context.Customers.Any(x => x.Id == orderHistory.CustomerID))
            {
                _logger.LogInformation("Adding new order history for customer ID: {CustomerId} and book ID: {BookId}.", orderHistory.CustomerID, orderHistory.BookID);
                _context.OrderHistories.Add(orderHistory);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetOrderHistory", new { id = orderHistory.Id }, orderHistory);
            }
            _logger.LogWarning("Failed to add order history: Book ID {BookId} or Customer ID {CustomerId} does not exist.", orderHistory.BookID, orderHistory.CustomerID);
            return BadRequest();
        }

        // DELETE: api/OrderHistories/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderHistory(int id)
        {
            _logger.LogInformation("Deleting order history with ID: {Id}", id);
            var orderHistory = await _context.OrderHistories.FindAsync(id);
            if (orderHistory == null)
            {
                _logger.LogWarning("Order history with ID: {Id} not found.", id);
                return NotFound();
            }

            _context.OrderHistories.Remove(orderHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order history with ID: {Id} deleted successfully.", id);
            return NoContent();
        }

        private bool OrderHistoryExists(int id)
        {
            return _context.OrderHistories.Any(e => e.Id == id);
        }
    }
}
