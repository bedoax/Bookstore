using System;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a review for a book.
    /// </summary>
    public class Review
    {

        public int Id { get; set; }


        [Required]
        public int BookID { get; set; }


        [Required]
        public int CustomerID { get; set; }


        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }


        [Required]
        [StringLength(255, ErrorMessage = "Comment length can't be more than 255.")]
        public string Comment { get; set; }


        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.Now;


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Likes must be a non-negative number.")]
        public int Likes { get; set; } = 0;


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Dislikes must be a non-negative number.")]
        public int Dislikes { get; set; } = 0;


        [StringLength(255, ErrorMessage = "Response length can't be more than 255.")]
        public string? Response { get; set; } = "";


        public DateTime? ResponseDate { get; set; } = DateTime.Now;

        public Book Book { get; set; }


        public Customer Customer { get; set; }
    }
}
