using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents a category of books in the bookstore.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Category name length can't be more than 100.")]
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the collection of books associated with the category.
        /// </summary>
        public ICollection<Book> Books { get; set; }
    }
}
