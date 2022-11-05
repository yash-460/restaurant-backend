using restaurantUtility.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using StoreManagementService.Models;
using restaurantUtility.Models;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> AuthenticateAsync(UserDTO user)
        {

            var registerdUser = _dbContext.Users.Find(user.UserName);
            if (registerdUser == null || !registerdUser.Password.Equals(user.Password))
                return Unauthorized();

            var token = await CreateToken(registerdUser);          
            return Ok(new { token = token });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync() {
            var registerdUser = _dbContext.Users.Find(User.Identity.Name);
            var token = await CreateToken(registerdUser);
            return Ok(new { token = token });
        }

        private async Task<String> CreateToken(User user)
        {
            var result = await _dbContext.UserRoles.Where(r => r.UserName == user.UserName).ToListAsync();

            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            foreach (var role in result)
            {
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Role));
                if (role.Role.Equals(Constants.STORE_OWNER_ROLE))
                    claimIdentity.AddClaim(new Claim("Store", user.StoreId.ToString()));
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
            return tokenHandler.WriteToken(token);
        }
    }
}
