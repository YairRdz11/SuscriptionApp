using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SuscriptionApp.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SuscriptionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountsController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser { 
                UserName = userCredentials.Email, 
                Email = userCredentials.Email 
            };
            var result = await userManager.CreateAsync(user, userCredentials.Password);
            
            if(result.Succeeded)
            {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager
                .PasswordSignInAsync(userCredentials.Email, userCredentials.Password, 
                isPersistent: false, lockoutOnFailure: false);

            if(result.Succeeded)
            {
                return BuildToken(userCredentials);
            }
            else
            {
                return BadRequest("Incorrect login");
            }
        }

        private AuthenticationResponse BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentials.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds =  new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, 
                claims: claims, expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }
    }
}
