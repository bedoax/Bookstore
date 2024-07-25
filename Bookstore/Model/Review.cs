using System;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a review for a book.
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Gets or sets the unique identifier for the review.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the book being reviewed.
        /// </summary>
        [Required]
        public int BookID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the customer who wrote the review.
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// Gets or sets the rating given by the customer, ranging from 1 to 5.
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the comment written by the customer.
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Comment length can't be more than 255.")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the review was created.
        /// </summary>
        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the number of likes the review has received.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Likes must be a non-negative number.")]
        public int Likes { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of dislikes the review has received.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Dislikes must be a non-negative number.")]
        public int Dislikes { get; set; } = 0;

        /// <summary>
        /// Gets or sets the response from the author or admin to the review.
        /// </summary>
        [StringLength(255, ErrorMessage = "Response length can't be more than 255.")]
        public string? Response { get; set; } = "";

        /// <summary>
        /// Gets or sets the date and time when the response was provided.
        /// </summary>
        public DateTime? ResponseDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property to the associated book.
        /// </summary>
        public Book Book { get; set; }

        /// <summary>
        /// Navigation property to the associated customer.
        /// </summary>
        public Customer Customer { get; set; }
    }
}
