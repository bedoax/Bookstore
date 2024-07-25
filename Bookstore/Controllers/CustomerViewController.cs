using Bookstore.Data;
using Bookstore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CustomerViewController(BookstoreDbContext context, JwtOptions jwt) : ControllerBase
    {
        [HttpPost]
        [Route("AuthCustomer")]
        public ActionResult AuthenticationAdmin(AuthenticationRequest authenticationRequset)
        {
            var customer = context.Customers.FirstOrDefault(x => x.Username == authenticationRequset.Username &&
            x.Password == authenticationRequset.Password);
            if (customer == null)
            {
                return Unauthorized();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDiscripor = new SecurityTokenDescriptor
            {
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier,customer.Id.ToString()),
                    new(ClaimTypes.Name,customer.Name),
                    new(ClaimTypes.Email,customer.Email),
                    new(ClaimTypes.MobilePhone,customer.PhoneNumber),
                    new(ClaimTypes.Gender,customer.Gender),
                    new(ClaimTypes.Country , customer.Country),
                    new(ClaimTypes.Role,"Customer")
                })
            };
            var securityToken = tokenHandler.CreateToken(tokenDiscripor);
            var access = tokenHandler.WriteToken(securityToken);
            return Ok(access);
        }
    }
}
