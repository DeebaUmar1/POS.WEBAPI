
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Repositories.UserRepository;
namespace POS.Services
{
   
    public class TokenServices 
    {
        private const string SecretKey = "YourSuperSecretKeyThatIsAtLeast32BytesLong!"; // Ensure this key is at least 32 bytes long
        private readonly SymmetricSecurityKey _signingKey;
       
       

        public TokenServices(IConfiguration configuration)
        {
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
            if (string.IsNullOrEmpty(SecretKey) || SecretKey.Length < 32)
            {
                throw new InvalidOperationException("The secret key must be at least 32 bytes long.");
            }
        }
        public string GenerateToken(LoginDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
           
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.Name, user.name),
                        new Claim(ClaimTypes.Role, user.role.ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            return principal;
        }
    }
}