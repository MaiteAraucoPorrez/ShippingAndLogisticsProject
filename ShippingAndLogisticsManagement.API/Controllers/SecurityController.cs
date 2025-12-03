using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Provides API endpoints for user security operations, including user registration.
    /// </summary>
    /// <remarks>This controller is accessible without authentication and is intended for scenarios where
    /// users need to register or perform other security-related actions anonymously. All endpoints are routed under
    /// 'api/security'.</remarks>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        public SecurityController(ISecurityService securityService, IMapper mapper, IPasswordService passwordService)
        {
            _securityService = securityService;
            _mapper = mapper;
            _passwordService = passwordService;
        }
        /// <summary>
        /// Registers a new user with the provided security information.
        /// </summary>
        /// <remarks>This action is accessible without authentication. The password is securely hashed
        /// before registration. The response includes the registered user's details, which may differ from the input
        /// due to processing or default values.</remarks>
        /// <param name="securityDto">The security data for the user to register. Must include a valid password and any required user details.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{SecurityDto}"/> with the registered
        /// user's security information.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(SecurityDto securityDto)
        {
            var security = _mapper.Map<Security>(securityDto);
            security.Password = _passwordService.Hash(securityDto.Password);

            await _securityService.RegisterUser(security);

            securityDto = _mapper.Map<SecurityDto>(security);
            var response = new ApiResponse<SecurityDto>(securityDto);
            return Ok(response);
        }
    }
}
