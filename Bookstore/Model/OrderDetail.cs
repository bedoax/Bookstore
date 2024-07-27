using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    public class OrderDetail
    {

        public int Id { get; set; }

        public int OrderHistoryId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }


        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0.")]
        public decimal TotalPrice { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "OrderStatus length can't be more than 50.")]
        public string OrderStatus { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "PaymentMethod length can't be more than 50.")]
        public string PaymentMethod { get; set; }


        [Required]
        [StringLength(255, ErrorMessage = "ShippingAddress length can't be more than 255.")]
        public string ShippingAddress { get; set; }


        [Required]
        [StringLength(255, ErrorMessage = "BillingAddress length can't be more than 255.")]
        public string BillingAddress { get; set; }


        public DateTime? DeliveryDate { get; set; }

        public OrderHistory OrderHistory { get; set; }
    }
}
