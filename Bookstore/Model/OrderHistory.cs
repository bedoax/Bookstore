using System;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents the history of an order placed by a customer.
    /// </summary>
    public class OrderHistory
    {
        // id book , id customer date

        public int Id { get; set; }


        [Required]
        public int CustomerID { get; set; }


        [Required]
        public int BookID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }


        public OrderDetail Detail { get; set; }
        public Customer Customer { get; set; }

        public Book Book { get; set; }
    }
}
