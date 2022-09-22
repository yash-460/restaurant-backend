using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly restaurantDBContext _dbContext;
        private readonly string _jwtSecret;
        public AuthenticateController(restaurantDBContext dBContext, IConfiguration config)
        {
            _dbContext = dBContext;
            _jwtSecret = config["JWT:key"];
        }

        /// <summary>
        /// This method/endpoint will create JWT token 
        /// </summary>
        /// <param name="user">object containing user_name and password</param>
        /// <returns>JWT token for the requested user with roles</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(UserDTO user)
        {

            var registerdUser = _dbContext.Users.Find(user.UserName);
            if (registerdUser == null || !registerdUser.Password.Equals(user.Password))
                return Unauthorized();

            var result = await  _dbContext.UserRoles.Where(r => r.UserName == user.UserName).ToListAsync();

            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            foreach (var role in result)
            {
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role));
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(token));
        }

    }
}
