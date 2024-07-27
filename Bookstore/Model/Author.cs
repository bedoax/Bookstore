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

        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }

   
        [Required]
        [StringLength(10, ErrorMessage = "Gender length can't be more than 10.")]
        public string Gender { get; set; }


        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
        public int Age { get; set; }


        [StringLength(100, ErrorMessage = "Country length can't be more than 100.")]
        public string Country { get; set; }


        [StringLength(500, ErrorMessage = "Description length can't be more than 500.")]
        public string Description { get; set; }


        [Phone]
        public string PhoneNumber { get; set; }


        [StringLength(100, ErrorMessage = "City length can't be more than 100.")]
        public string City { get; set; }


        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email length can't be more than 255.")]
        public string Email { get; set; }


        public string ImagePath { get; set; } = "";

        [Url]
        public string Website { get; set; }


        public ICollection<Book> Books { get; set; }
    }
}
