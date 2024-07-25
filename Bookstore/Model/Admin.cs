using System.ComponentModel.DataAnnotations;

namespace Bookstore.Model
{
    /// <summary>
    /// Represents an administrator in the bookstore system.
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Gets or sets the unique identifier for the administrator.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the administrator. It is required and cannot exceed 50 characters.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Username length can't be more than 50.")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the administrator. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Password length can't be more than 100.")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the full name of the administrator. It is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the administrator. It should be a valid phone number format.
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address of the administrator. It should be a valid email format and cannot exceed 255 characters.
        /// </summary>
        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email length can't be more than 255.")]
        public string Email { get; set; }
    }
}
