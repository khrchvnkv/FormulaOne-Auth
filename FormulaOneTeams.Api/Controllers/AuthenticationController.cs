using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FormulaOneTeams.Api.Configurations;
using FormulaOneTeams.Api.Models;
using FormulaOneTeams.Api.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FormulaOneTeams.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            // validate the incoming request
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(requestDto.Email);

                if (userExist is not null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }

                // create a user
                var newUser = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email
                };

                var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);
                if (isCreated.Succeeded)
                {
                    // Generate token
                    var token = GenerateToken(newUser);

                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }

                var errorsList = new List<string>();
                foreach (var error in isCreated.Errors)
                {
                    errorsList.Add($"Error - {error.Code}:{error.Description}");
                }
                
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = errorsList
                });
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (existingUser is null) return GetBadRequestResult();

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);
                if (!isCorrect) return GetBadRequestResult();

                var jwtToken = GenerateToken(existingUser);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = jwtToken
                });
            }

            return GetBadRequestResult();

            IActionResult GetBadRequestResult()
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            // Token descriptor  
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}