using Bookstore.Data;
using Bookstore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CustomerViewController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly JwtOptions _jwt;
        private readonly ILogger<CustomerViewController> _logger;

        public CustomerViewController(BookstoreDbContext context, JwtOptions jwt, ILogger<CustomerViewController> logger)
        {
            _context = context;
            _jwt = jwt;
            _logger = logger;
        }

        [HttpPost]
        [Route("AuthCustomer")]
        public ActionResult AuthenticationCustomer(AuthenticationRequest authenticationRequest)
        {
            _logger.LogInformation("Authentication attempt for username: {Username}", authenticationRequest.Username);

            var customer = _context.Customers.FirstOrDefault(x => x.Username == authenticationRequest.Username &&
            x.Password == authenticationRequest.Password);

            if (customer == null)
            {
                _logger.LogWarning("Authentication failed for username: {Username}", authenticationRequest.Username);
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                    new(ClaimTypes.Name, customer.Name),
                    new(ClaimTypes.Email, customer.Email),
                    new(ClaimTypes.MobilePhone, customer.PhoneNumber),
                    new(ClaimTypes.Gender, customer.Gender),
                    new(ClaimTypes.Country, customer.Country),
                    new(ClaimTypes.Role, "Customer")
                })
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            _logger.LogInformation("Authentication successful for username: {Username}", authenticationRequest.Username);
            return Ok(accessToken);
        }
    }
}
