namespace Bookstore.Model
{
    /// <summary>
    /// Represents the request payload for user authentication.
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Gets or sets the username for authentication. It is required and should be in a valid format.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication. It is required and should be secure.
        /// </summary>
        public string Password { get; set; }
    }
}
