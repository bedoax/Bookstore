using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{

    public class Category
    {

        public int Id { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Category name length can't be more than 100.")]
        public string CategoryName { get; set; }


        public ICollection<Book> Books { get; set; }
    }
}
