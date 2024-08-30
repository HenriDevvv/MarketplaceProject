using Domain.Contracts;
using Domain.UoW;
using DTO.UserDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidateController : ControllerBase
    {
        private IDomainUnitOfWork _domainUnitOfWork;
        private IConfiguration _configuration;
        public ValidateController(IDomainUnitOfWork domainUnitOfWork, IConfiguration configuration)
        {
            _domainUnitOfWork = domainUnitOfWork;
            _configuration = configuration;
        }
        private IUserDomain _userDomain => _domainUnitOfWork.GetDomain<IUserDomain>();
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserLoginDTO dto)
        {
            var user = _userDomain.GetUserByUsername(dto.Username);
            if (user != null && _userDomain.CheckPassword(user, dto.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var role in user.Roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var token = GetToken(authClaims);
                return Ok(
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
            }
            return Unauthorized();
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
