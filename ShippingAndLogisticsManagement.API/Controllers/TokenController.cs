using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for user authentication and configuration retrieval, including issuing JWT tokens and
    /// exposing application connection settings.
    /// </summary>
    /// <remarks>This controller enables clients to authenticate users and obtain JWT tokens for secure access
    /// to protected resources. It also provides endpoints for retrieving configuration details such as connection
    /// strings and environment information. All authentication operations are performed using injected security and
    /// password services. Endpoints are designed for use in web applications requiring token-based authentication and
    /// configuration diagnostics.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;
        public TokenController(IConfiguration configuration, ISecurityService securityService, IPasswordService passwordService)
        {
            _securityService = securityService;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Authenticates a user based on the provided login credentials and returns a JWT token if authentication is
        /// successful.
        /// </summary>
        /// <remarks>This endpoint allows anonymous access and is typically used to obtain an
        /// authentication token for subsequent requests. The returned token should be included in the Authorization
        /// header of future API calls.</remarks>
        /// <param name="userLogin">An object containing the user's login credentials. Must include valid username and password information.</param>
        /// <returns>An IActionResult containing a JWT token if authentication succeeds; otherwise, an Unauthorized result.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authentication(UserLogin userLogin)
        {
            //Si es un usuario válido
            var validation = await IsValidUser(userLogin);

            if (validation.Item1)
            {
                var token = GenerateToken(validation.Item2);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private async Task<(bool, Security)> IsValidUser(UserLogin userLogin)
        {
            var user = await _securityService.GetLoginByCredentials(userLogin);
            var isValidHash = _passwordService.VerifyPassword(user.Password, userLogin.Password);
            return (isValidHash, user);
        }

        private string GenerateToken(Security security)
        {
            string secretKey = _configuration["Authentication:SecretKey"];

            // Header
            var symetricSecurityKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = 
                new SigningCredentials(symetricSecurityKey, 
                SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // Body Payload
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, security.Name),
                new Claim("Login", security.Login),
                new Claim(ClaimTypes.Role, security.Role.ToString())
            };
            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(500)
            );

            // Firma
            var token = new JwtSecurityToken(header, payload);

            // Serializer
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        /// <summary>
        /// Handles an HTTP GET request to retrieve the SQL Server connection string for the current configuration.
        /// </summary>
        /// <remarks>This endpoint is intended for diagnostic or configuration purposes and returns
        /// sensitive connection information. Use with caution and restrict access appropriately.</remarks>
        /// <param name="userLogin">The user login information provided with the request. This parameter is not used in the response.</param>
        /// <returns>An <see cref="IActionResult"/> containing an object with the SQL Server connection string from the
        /// application's configuration.</returns>
        [HttpGet("Test")]
        public async Task<IActionResult> Test(UserLogin userLogin)
        {
            var result = new
            {
                ConeccionSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"],
            };

            return Ok(result);
        }


        /// <summary>
        /// Handles a GET request to test the SQL Server connection configuration and returns the connection string
        /// information.
        /// </summary>
        /// <remarks>This endpoint is intended for diagnostic purposes and does not attempt to establish a
        /// database connection. It simply returns the configured connection string value. Use caution when exposing
        /// connection string information, as it may contain sensitive data.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the SQL Server connection string information if successful;
        /// otherwise, an error response with status code 500.</returns>
        [HttpGet("TestConeccion")]
        public async Task<IActionResult> TestConeccion()
        {
            try
            {
                var result = new
                {
                    ConnectionSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"]
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }


        /// <summary>
        /// Retrieves configuration details for the application, including connection strings, environment information,
        /// and authentication settings.
        /// </summary>
        /// <remarks>The returned object includes the MySQL and SQL Server connection strings, a list of
        /// all connection strings, the current ASP.NET Core environment, and authentication settings. If a
        /// configuration value is missing, a default message is provided in its place.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a JSON object with the application's configuration details.
        /// Returns a 500 Internal Server Error response if an exception occurs.</returns>
        [HttpGet("Config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var connectionStringMySql = _configuration["ConnectionStrings:ConnectionMySql"];
                var connectionStringSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"];

                var result = new
                {
                    connectionStringMySql = connectionStringMySql ?? "My SQL NO CONFIGURADO",
                    connectionStringSqlServer = connectionStringSqlServer ?? "SQL SERVER NO CONFIGURADO",
                    AllConnectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren().Select(x => new { Key = x.Key, Value = x.Value }),
                    Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "ASPNETCORE_ENVIRONMENT NO CONFIGURADO",
                    Authentication = _configuration.GetSection("Authentication").GetChildren().Select(x => new { Key = x.Key, Value = x.Value })
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }
    }       
}
