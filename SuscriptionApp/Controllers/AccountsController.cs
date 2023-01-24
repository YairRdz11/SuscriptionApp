using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
                return await BuildToken(userCredentials, user.Id);
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
                var user = await userManager.FindByEmailAsync(userCredentials.Email);
                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest("Incorrect login");
            }
        }
        [HttpGet("rebuild-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Renovate()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var idClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var userId = idClaim.Value;

            var userCredentials = new UserCredentials()
            {
                Email = emailClaim.Value
            };
            return await BuildToken(userCredentials, userId);
        }

        [HttpPost("make-admin")]
        public async Task<ActionResult> MakeAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));

            return NoContent();
        }

        [HttpPost("remove-admin")]
        public async Task<ActionResult> RemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));

            return NoContent();
        }

        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials,
            string userId)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentials.Email),
                new Claim("id", userId)
            };

            var user = await userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

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
