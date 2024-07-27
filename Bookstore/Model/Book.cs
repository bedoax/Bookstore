using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a book in the bookstore.
    /// </summary>
    public class Book
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title { get; set; }


        [Required(ErrorMessage = "AuthorId is required.")]
        public int AuthorId { get; set; }


        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters.")]
        public string ISBN { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "CategoryID is required.")]
        public int CategoryID { get; set; }


        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "PublishedDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime PublishedDate { get; set; }


        [StringLength(100, ErrorMessage = "Publisher cannot be longer than 100 characters.")]
        public string Publisher { get; set; }


        [StringLength(50, ErrorMessage = "Language cannot be longer than 50 characters.")]
        public string Language { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Number of pages must be at least 1.")]
        public int NumberOfPages { get; set; }


        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public decimal Rating { get; set; }

        public string ImagePath { get; set; } = "";

        public string URL { get; set; } = "";

        // Navigation Properties


        public Author Author { get; set; }


        public Category Category { get; set; }


        public ICollection<OrderHistory> OrderHistories { get; set; }


        public ICollection<Review> Reviews { get; set; }
    }
}
