using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public CustomerController(BookstoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            if(!customers.Any() || customers == null)
            {
                return BadRequest();
            }
            return customers;
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if(_context.Customers.Any(x=>x.Username == customer.Username && x.Password == customer.Password))
            {
                return BadRequest();
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("BuyBooks")]
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> BuyBooks(
        [FromBody] List<BookPurchaseRequest> bookPurchases,
        [FromQuery] int customerId,
        [FromQuery] string paymentMethod,
        [FromQuery] string shippingAddress,
        [FromQuery] string billingAddress,
        [FromQuery] DateTime deliveryDate)
        {
            // Start transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Check if customer exists
                    var customer = await _context.Customers.FindAsync(customerId);
                    if (customer == null)
                    {
                        return NotFound("Customer not found.");
                    }

                    decimal totalOrderPrice = 0;

                    foreach (var bookPurchase in bookPurchases)
                    {
                        // Check if book exists (case-insensitive comparison)
                        var book = await _context.Books
                            .AsNoTracking()
                            .FirstOrDefaultAsync(b => b.Title.ToLower() == bookPurchase.BookTitle.ToLower());
                        if (book == null)
                        {
                            return NotFound($"Book '{bookPurchase.BookTitle}' not found.");
                        }

                        // Check if book is in stock
                        if (book.Quantity < bookPurchase.Quantity)
                        {
                            return BadRequest($"Not enough stock available for book '{bookPurchase.BookTitle}'.");
                        }

                        // Calculate total price for the book
                        var bookTotalPrice = book.Price * bookPurchase.Quantity;
                        totalOrderPrice += bookTotalPrice;

                        // Decrease book quantity
                        book.Quantity -= bookPurchase.Quantity;
                        _context.Entry(book).State = EntityState.Modified;

                        // Add entry to OrderHistory
                        var orderHistory = new OrderHistory
                        {
                            CustomerID = customer.Id,
                            BookID = book.Id,
                            OrderDate = DateTime.UtcNow,
                            Quantity = bookPurchase.Quantity,
                            TotalPrice = bookTotalPrice,
                            OrderStatus = "Completed",
                            PaymentMethod = paymentMethod,
                            ShippingAddress = shippingAddress,
                            BillingAddress = billingAddress,
                            DeliveryDate = deliveryDate
                        };
                        _context.OrderHistories.Add(orderHistory);
                    }

                    // Check if customer has enough balance for total order price
                    if (customer.Balance < totalOrderPrice)
                    {
                        return BadRequest("Insufficient balance for the total order.");
                    }

                    // Update customer's balance
                    customer.Balance -= totalOrderPrice;
                    _context.Entry(customer).State = EntityState.Modified;

                    // Save changes to database
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return Ok("Books purchased successfully.");
                }
                catch (Exception ex)
                {
                    // Rollback transaction if any error occurs
                    await transaction.RollbackAsync();

                    // Log the exception and its details
                    // _logger.LogError(ex, "An error occurred while processing the purchase.");
                    return StatusCode(500, $"Internal server error: {ex.Message}. Inner exception: {ex.InnerException?.Message}");
                }
            }
        }






        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
