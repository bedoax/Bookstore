using System;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents the history of an order placed by a customer.
    /// </summary>
    public class OrderHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order history record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the customer who placed the order.
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the book being ordered.
        /// </summary>
        [Required]
        public int BookID { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the order was placed.
        /// </summary>
        [Required]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the book ordered. Must be at least 1.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the total price of the order. Must be greater than 0.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0.")]
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the status of the order.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "OrderStatus length can't be more than 50.")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the payment method used for the order.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "PaymentMethod length can't be more than 50.")]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping address for the order.
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "ShippingAddress length can't be more than 255.")]
        public string ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing address for the order.
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "BillingAddress length can't be more than 255.")]
        public string BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the order was delivered.
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Navigation property to the associated customer.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Navigation property to the associated book.
        /// </summary>
        public Book Book { get; set; }
    }
}
