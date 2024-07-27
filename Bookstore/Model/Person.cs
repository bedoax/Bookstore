using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    public abstract class Person
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username length can't be more than 50.")]
        public string Username { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Password length can't be more than 100.")]
        public string Password { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }


        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email length can't be more than 255.")]
        public string Email { get; set; }
    }
}
