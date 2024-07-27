namespace Bookstore.Model
{
    /// <summary>
    /// Represents the request payload for user authentication.
    /// </summary>
    public class AuthenticationRequest
    {

        public string Username { get; set; }


        public string Password { get; set; }
    }
}
