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
    public class CustomerController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(BookstoreDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            _logger.LogInformation("Fetching all customers.");
            var customers = await _context.Customers.ToListAsync();
            if (!customers.Any() || customers == null)
            {
                _logger.LogWarning("No customers found.");
                return BadRequest();
            }
            _logger.LogInformation("Retrieved {Count} customers.", customers.Count);
            return Ok(customers);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            _logger.LogInformation("Fetching customer with ID {Id}.", id);
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved customer with ID {Id}.", id);
            return Ok(customer);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                _logger.LogWarning("Customer ID in the URL ({UrlId}) does not match the customer ID in the body ({BodyId}).", id, customer.Id);
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated customer with ID {Id}.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    _logger.LogWarning("Customer with ID {Id} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception occurred while updating customer with ID {Id}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customers.Any(x => x.Username == customer.Username && x.Password == customer.Password))
            {
                _logger.LogWarning("Customer with username {Username} already exists.", customer.Username);
                return BadRequest();
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new customer with ID {Id}.", customer.Id);

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation("Deleting customer with ID {Id}.", id);
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted customer with ID {Id}.", id);

            return NoContent();
        }

        [HttpPost("BuyBooks")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BuyBooks(
            [FromBody] List<BookPurchaseRequest> bookPurchases,
            [FromQuery] int customerId,
            [FromQuery] string paymentMethod,
            [FromQuery] string shippingAddress,
            [FromQuery] string billingAddress,
            [FromQuery] DateTime deliveryDate)
        {
            _logger.LogInformation("Customer {CustomerId} is attempting to buy books.", customerId);
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = await _context.Customers.FindAsync(customerId);
                    if (customer == null)
                    {
                        _logger.LogWarning("Customer with ID {CustomerId} not found.", customerId);
                        return NotFound("Customer not found.");
                    }

                    decimal totalOrderPrice = 0;

                    foreach (var bookPurchase in bookPurchases)
                    {
                        var book = await _context.Books
                            .AsNoTracking()
                            .FirstOrDefaultAsync(b => b.Title.ToLower() == bookPurchase.BookTitle.ToLower());
                        if (book == null)
                        {
                            _logger.LogWarning("Book '{BookTitle}' not found.", bookPurchase.BookTitle);
                            return NotFound($"Book '{bookPurchase.BookTitle}' not found.");
                        }

                        if (book.Quantity < bookPurchase.Quantity)
                        {
                            _logger.LogWarning("Not enough stock available for book '{BookTitle}'.", bookPurchase.BookTitle);
                            return BadRequest($"Not enough stock available for book '{bookPurchase.BookTitle}'.");
                        }

                        var bookTotalPrice = book.Price * bookPurchase.Quantity;
                        totalOrderPrice += bookTotalPrice;

                        book.Quantity -= bookPurchase.Quantity;
                        _context.Entry(book).State = EntityState.Modified;

                        var orderHistory = new OrderHistory
                        {
                            CustomerID = customer.Id,
                            BookID = book.Id,
                            OrderDate = DateTime.UtcNow
                        };
                        _context.OrderHistories.Add(orderHistory);

                        var orderDetail = new OrderDetail
                        {
                            OrderHistoryId = orderHistory.Id,
                            Quantity = bookPurchase.Quantity,
                            TotalPrice = bookTotalPrice,
                            OrderStatus = "Pending",
                            PaymentMethod = paymentMethod,
                            ShippingAddress = shippingAddress,
                            BillingAddress = billingAddress,
                            DeliveryDate = deliveryDate
                        };
                        _context.OrderDetails.Add(orderDetail);
                    }

                    if (customer.Balance < totalOrderPrice)
                    {
                        _logger.LogWarning("Customer {CustomerId} has insufficient balance for the total order.", customerId);
                        return BadRequest("Insufficient balance for the total order.");
                    }

                    customer.Balance -= totalOrderPrice;
                    _context.Entry(customer).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Customer {CustomerId} successfully purchased books.", customerId);
                    return Ok("Books purchased successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "An error occurred while processing the purchase for customer {CustomerId}.", customerId);
                    return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
                }
            }
        }

        private bool CustomerExists(int id)
        {
            bool exists = _context.Customers.Any(e => e.Id == id);
            _logger.LogInformation("Customer with ID {Id} exists: {Exists}.", id, exists);
            return exists;
        }
    }
}
