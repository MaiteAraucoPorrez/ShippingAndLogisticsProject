using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        public TokenController(IConfiguration configuration, ISecurityService securityService)
        {
            _securityService = securityService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(UserLogin userLogin)
        {
            //Si es un usuario válido
            var validation = await IsValidUser(userLogin);
            if (validation.Item1)
            {
                var token = GenerateToken(validation.Item2);
                return Ok(new { token });
            }

            return NotFound();
        }

        private async Task<(bool, Security)> IsValidUser(UserLogin userLogin)
        {
            var user = await _securityService.GetLoginByCredentials(userLogin);
            return (user != null, user);
        }

        private string GenerateToken(Security security)
        {
            string secretKey = _configuration["Authentication: SecretKey"];

            // Header
            var symetricSecurityKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = 
                new SigningCredentials(symetricSecurityKey, 
                SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // Body Payload
            var claim = new[]
            {
                new Claim("Name", security.Name),
                new Claim("Login", security.Login),
                new Claim(ClaimTypes.Role, security.Role.ToString())
            };
            var payload = new JwtPayload(
                issuer: _configuration["Authentication: Issuer"],
                audience: _configuration["Authentication: Audience"],
                claims: claim,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(3)
            );

            // Firma
            var token = new JwtSecurityToken(header, payload);

            // Serializer
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }       
}
