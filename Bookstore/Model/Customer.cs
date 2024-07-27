using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a customer in the bookstore system.
    /// </summary>
    public class Customer: Person
    {


        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a non-negative value.")]
        public decimal Balance { get; set; }

        [StringLength(10, ErrorMessage = "Gender length can't be more than 10.")]
        public string Gender { get; set; }


        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
        public int Age { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Country length can't be more than 100.")]
        public string Country { get; set; }


        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }



        [StringLength(100, ErrorMessage = "City length can't be more than 100.")]
        public string City { get; set; }


        [StringLength(100, ErrorMessage = "Street length can't be more than 100.")]
        public string Street { get; set; }


        public ICollection<OrderHistory> OrderHistories { get; set; }


        public ICollection<Review> Reviews { get; set; }
    }
}
