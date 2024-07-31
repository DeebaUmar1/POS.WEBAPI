using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services;
using POS.Services.UserServices;
using POS.Validation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static POS.Middlewares.Middlewares.CustomExceptions;

namespace POS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userServices;   
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration _configuration, IUserService userServices, IMapper mapper, ILogger<AuthenticationController> logger)
        {
            this._configuration = _configuration;
            _userServices = userServices;
          
            _mapper = mapper;
            _logger = logger;
        }

  
        //To store users
       // [Authorize]
        [HttpPost("SeedUsers")]
        public async Task<IActionResult> SeedUsers()
        {
            try
            {
                await _userServices.SeedUsers();
                _logger.LogInformation("Users added successfully!");
                return Ok("Users seeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while seeding users: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }
        //To get a user by id
        [HttpPost("GetUserByID")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userServices.GetUserById(id);
                _logger.LogInformation("Users added successfully!");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while seeding users: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //Login a user
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                var user = await _userServices.Login(username, password);
                // this will return if name/password is incorrect
                if (user == null)
                {
                    throw new UnauthorizedAccessEx("Invalid credentials");
                }

                var loggedInUser = _mapper.Map<LoginDTO>(user);
                //To authorize users based on their roles
                var authClaims = new List<Claim>
                {
                   new Claim(ClaimTypes.Name, loggedInUser.name),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim(ClaimTypes.Role, loggedInUser.role.ToString())
                };
           
                string token = GenerateToken(authClaims);
                _logger.LogInformation("User logged in!");
                return Ok(new { message = token });
      
            }
            catch (UnauthorizedAccessEx ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while logging in: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                //Check if the model is invalid
                if (string.IsNullOrEmpty(user.name) || string.Equals(user.name, "string"))
                {
                    throw new ValidationException("Invalid Name");
                }

                if (string.IsNullOrEmpty(user.email) || string.Equals(user.email, "string") || !EmailValidation.IsValidEmail(user.email))
                {
                    throw new ValidationException("Invalid Email");
                }
                //Validate password
                Regex validatePassword = Password.ValidatePassword();
                if (string.IsNullOrEmpty(user.password) || !validatePassword.IsMatch(user.password))
                {
                    throw new ValidationException("Invalid Password (must contain numbers, at least one capital letter)");
                }
                //Encoding the password
                user.password = Password.EncodePasswordToBase64(user.password);
                //Default Role
                user.role = UserRole.Cashier;

                //Mapping LoginDTO to User entity
                var userEntity = _mapper.Map<User>(user);

                if (await _userServices.RegisterUserAsync(userEntity))
                {
                    _logger.LogInformation("User registered successfully!");
                    return Ok("User registered successfully as a Cashier");
                }
                else
                {
                    throw new ValidationException("User already exists!");
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"Validation error during registration: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("setrole")]
        public async Task<IActionResult> SetRole([FromBody] SetRoleModelDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                //in case of Basic Authentication this code will be uncommented
               /* var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    throw new UnauthorizedAccessEx("Unauthorized access");
                }

                var user = await _userServices.GetUserById(int.Parse(userId));
                if (user == null || user.role != UserRole.Admin)
                {
                    throw new UnauthorizedAccessEx("Only admins can update roles");
                }*/
                //Check user input
                UserRole role;
                if (model.role.ToLower() == "admin")
                {
                    role = UserRole.Admin;
                }
                else if( model.role.ToLower() == "cashier")
                {
                  role = UserRole.Cashier;
                }
                else
                {
                    throw new Exception("Invalid role");
                }
                var success = await _userServices.UpdateUserRole(model.username, role);
                if (!success)
                {
                    throw new NotFoundException("User not found");
                }

                return Ok(new { message = "User role updated successfully" });
            }
            catch (UnauthorizedAccessEx ex)
            {
                _logger.LogError($"Authorization error: {ex.Message}");
                return Unauthorized(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($"Not found error: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while setting role: {ex.Message}");
                throw;// Rethrow to be caught by middleware
            }
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userServices.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting users: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }
        
        // This code will generate token for individual users based on their roles
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
