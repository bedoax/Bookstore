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
    [Route("api/[controller]")]
    public class AdminViewController(BookstoreDbContext context,JwtOptions jwt):ControllerBase
    {
        [HttpPost]
        [Route("Auth")]
        public ActionResult AuthenticationAdmin(AuthenticationRequest authenticationRequset)
        {
            var admin = context.Admins.FirstOrDefault(x=>x.Username == authenticationRequset.Username &&
            x.Password == authenticationRequset.Password);
            if (admin == null)
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
                    new(ClaimTypes.NameIdentifier,admin.Id.ToString()),
                    new(ClaimTypes.Name,admin.Name),
                    new(ClaimTypes.Email,admin.Email),
                    new(ClaimTypes.MobilePhone,admin.PhoneNumber),
                    new(ClaimTypes.Role,"Admin")
                })
            };
            var securityToken = tokenHandler.CreateToken(tokenDiscripor);
            var access = tokenHandler.WriteToken(securityToken);
            return Ok(access);
        }
    }
}
