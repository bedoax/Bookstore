using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a customer in the bookstore system.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the customer. It is required and must be at most 50 characters long.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Username length can't be more than 50.")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address of the customer. It is required, must be a valid email address, and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email length can't be more than 100.")]
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the password of the customer. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Password length can't be more than 100.")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the balance of the customer's account. It must be a non-negative value.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a non-negative value.")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the gender of the customer. It can be up to 10 characters long.
        /// </summary>
        [StringLength(10, ErrorMessage = "Gender length can't be more than 10.")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the age of the customer. It must be between 0 and 150.
        /// </summary>
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the country where the customer resides. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Country length can't be more than 100.")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a description about the customer. It can be up to 500 characters long.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the customer. It must be a valid phone number and cannot exceed 20 characters.
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20, ErrorMessage = "PhoneNumber length can't be more than 20.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the city where the customer resides. It can be up to 100 characters long.
        /// </summary>
        [StringLength(100, ErrorMessage = "City length can't be more than 100.")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the street address of the customer. It can be up to 100 characters long.
        /// </summary>
        [StringLength(100, ErrorMessage = "Street length can't be more than 100.")]
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the collection of order histories associated with the customer.
        /// </summary>
        public ICollection<OrderHistory> OrderHistories { get; set; }

        /// <summary>
        /// Gets or sets the collection of reviews written by the customer.
        /// </summary>
        public ICollection<Review> Reviews { get; set; }
    }
}
