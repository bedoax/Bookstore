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
        /// <summary>
        /// Gets or sets the unique identifier for the book.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book. It is required and must be between 2 and 100 characters.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 100 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the author of the book. It is required.
        /// </summary>
        [Required(ErrorMessage = "AuthorId is required.")]
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the International Standard Book Number (ISBN) for the book. It is required and must be between 10 and 13 characters.
        /// </summary>
        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters.")]
        public string ISBN { get; set; }

        /// <summary>
        /// Gets or sets the description of the book. It cannot be longer than 500 characters.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the price of the book. It is required and must be between 0.01 and 10,000.
        /// </summary>
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the category of the book. It is required.
        /// </summary>
        [Required(ErrorMessage = "CategoryID is required.")]
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the book in stock. It is required and cannot be negative.
        /// </summary>
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the published date of the book. It is required and must be a valid date.
        /// </summary>
        [Required(ErrorMessage = "PublishedDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// Gets or sets the publisher of the book. It cannot be longer than 100 characters.
        /// </summary>
        [StringLength(100, ErrorMessage = "Publisher cannot be longer than 100 characters.")]
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the language of the book. It cannot be longer than 50 characters.
        /// </summary>
        [StringLength(50, ErrorMessage = "Language cannot be longer than 50 characters.")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the number of pages in the book. It must be at least 1.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Number of pages must be at least 1.")]
        public int NumberOfPages { get; set; }

        /// <summary>
        /// Gets or sets the rating of the book. It is required and must be between 0 and 5.
        /// </summary>
        [Required]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets the path to the cover image of the book.
        /// </summary>
        public string ImagePath { get; set; } = "";

        /// <summary>
        /// Gets or sets the URL for more information about the book.
        /// </summary>
        public string URL { get; set; } = "";

        // Navigation Properties

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// Gets or sets the category of the book.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the collection of order histories for the book.
        /// </summary>
        public ICollection<OrderHistory> OrderHistories { get; set; }

        /// <summary>
        /// Gets or sets the collection of reviews for the book.
        /// </summary>
        public ICollection<Review> Reviews { get; set; }
    }
}
