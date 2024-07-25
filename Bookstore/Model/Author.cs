using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents an author in the bookstore.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Gets or sets the unique identifier for the author.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the author. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the gender of the author. It is required and cannot exceed 10 characters.
        /// </summary>
        [Required]
        [StringLength(10, ErrorMessage = "Gender length can't be more than 10.")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the age of the author. It must be between 0 and 150.
        /// </summary>
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the country of the author. It cannot exceed 100 characters.
        /// </summary>
        [StringLength(100, ErrorMessage = "Country length can't be more than 100.")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a brief description about the author. It cannot exceed 500 characters.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the author. It should be a valid phone number format.
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the city of the author. It cannot exceed 100 characters.
        /// </summary>
        [StringLength(100, ErrorMessage = "City length can't be more than 100.")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the email address of the author. It should be a valid email format and cannot exceed 255 characters.
        /// </summary>
        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email length can't be more than 255.")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the path to the author's image.
        /// </summary>
        public string ImagePath { get; set; } = "";

        /// <summary>
        /// Gets or sets the website URL of the author. It should be a valid URL format.
        /// </summary>
        [Url]
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the collection of books written by the author.
        /// </summary>
        public ICollection<Book> Books { get; set; }
    }
}
