using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using ShippingAndLogisticsManagement.Infrastructure.Validator;
using System.Net;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Controller for address management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all address-related operations including CRUD operations,
    /// filtering by customer, city, department, and managing default addresses.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public AddressController(
            IAddressService addressService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _addressService = addressService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de direcciones con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por cliente, ciudad, departamento, tipo y estado.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/address/dto/mapper?CustomerId=5&amp;City=La Paz&amp;Type=Delivery&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de direcciones</returns>
        /// <response code="200">Retorna la lista paginada de direcciones</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran direcciones con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<AddressDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllAddresses([FromQuery] AddressQueryFilter filters)
        {
            try
            {
                var addresses = await _addressService.GetAllAsync(filters);
                var addressesDto = _mapper.Map<IEnumerable<AddressDto>>(addresses.Pagination);
                var pagination = new Pagination
                {
                    TotalCount = addresses.Pagination.TotalCount,
                    PageSize = addresses.Pagination.PageSize,
                    CurrentPage = addresses.Pagination.CurrentPage,
                    TotalPages = addresses.Pagination.TotalPages,
                    HasNextPage = addresses.Pagination.HasNextPage,
                    HasPreviousPage = addresses.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<AddressDto>>(addressesDto)
                {
                    Pagination = pagination,
                    Messages = addresses.Messages
                };

                return StatusCode((int)addresses.StatusCode, response);
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<AddressDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetAddressesDapper()
        {
            var result = await _addressService.GetAllDapperAsync();
            var dtos = _mapper.Map<IEnumerable<AddressDto>>(result);
            return Ok(new ApiResponse<IEnumerable<AddressDto>>(dtos));
        }

        /// <summary>
        /// Obtiene todas las direcciones de un cliente específico
        /// </summary>
        /// <remarks>
        /// Consulta optimizada que retorna todas las direcciones activas de un cliente,
        /// ordenadas por dirección predeterminada primero.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/address/dto/mapper/customer/5
        /// </remarks>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de direcciones del cliente</returns>
        /// <response code="200">Retorna las direcciones del cliente</response>
        /// <response code="400">Si el ID del cliente es inválido</response>
        /// <response code="404">Si el cliente no existe o no tiene direcciones</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<AddressDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/customer/{customerId}")]
        public async Task<IActionResult> GetAddressesByCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del cliente debe ser mayor a 0" } }
                    });
                }

                var addresses = await _addressService.GetByCustomerIdAsync(customerId);
                var addressesDto = _mapper.Map<IEnumerable<AddressDto>>(addresses);

                var response = new ApiResponse<IEnumerable<AddressDto>>(addressesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se recuperaron {addressesDto.Count()} direcciones del cliente" } }
                };

                return Ok(response);
            }
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Obtiene una dirección específica por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa de la dirección.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/address/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador único de la dirección</param>
        /// <returns>Información detallada de la dirección</returns>
        /// <response code="200">Retorna la dirección solicitada</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si la dirección no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AddressDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID debe ser mayor a 0" } }
                    });
                }

                var address = await _addressService.GetByIdAsync(id);
                var addressDto = _mapper.Map<AddressDto>(address);

                var response = new ApiResponse<AddressDto>(addressDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Dirección recuperada exitosamente" } }
                };

                return Ok(response);
            }
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<AddressDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper/{id}")]
        public async Task<IActionResult> GetAddressByIdDapper(int id)
        {
            var result = await _addressService.GetByIdDapperAsync(id);
            if (result == null) return NotFound();
            var dto = _mapper.Map<AddressDto>(result);
            return Ok(new ApiResponse<AddressDto>(dto));
        }

        /// <summary>
        /// Obtiene la dirección predeterminada de un cliente según el tipo
        /// </summary>
        /// <remarks>
        /// Retorna la dirección marcada como predeterminada para recogida o entrega.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/address/customer/5/default/Delivery
        /// </remarks>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="type">Tipo de dirección: Pickup o Delivery</param>
        /// <returns>Dirección predeterminada del cliente</returns>
        /// <response code="200">Retorna la dirección predeterminada</response>
        /// <response code="400">Si los parámetros son inválidos</response>
        /// <response code="404">Si el cliente no tiene dirección predeterminada de ese tipo</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AddressDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("customer/{customerId}/default/{type}")]
        public async Task<IActionResult> GetDefaultAddress(int customerId, string type)
        {
            try
            {
                if (customerId <= 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del cliente debe ser mayor a 0" } }
                    });
                }

                if (!Enum.TryParse<AddressType>(type, true, out var addressType))
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El tipo debe ser 'Pickup' o 'Delivery'" } }
                    });
                }

                var address = await _addressService.GetDefaultAddressAsync(customerId, addressType);
                var addressDto = _mapper.Map<AddressDto>(address);

                var response = new ApiResponse<AddressDto>(addressDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Dirección predeterminada recuperada exitosamente" } }
                };

                return Ok(response);
            }
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Crea una nueva dirección
        /// </summary>
        /// <remarks>
        /// Este método valida que el cliente exista y que todos los campos cumplan
        /// con las reglas de negocio. Aplica validaciones como:
        /// - Cliente debe existir
        /// - Dirección entre 5 y 200 caracteres
        /// - Departamento debe ser válido (uno de los 9 de Bolivia)
        /// - Máximo 10 direcciones activas por cliente
        /// - Si es predeterminada, desmarca las demás del mismo tipo
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/address
        ///{
        ///  "customerId": 5,
        ///  "street": "Av. 6 de Agosto #2170, Edificio Torres del Poeta",
        ///  "city": "La Paz",
        ///  "department": "La Paz",
        ///  "zone": "Sopocachi",
        ///  "type": "Delivery",
        ///  "isDefault": true,
        ///  "reference": "Edificio azul, tercer piso, portón negro",
        ///  "contactName": "Juan Pérez",
        ///  "contactPhone": "71234567",
        ///  "isActive": true
        ///}
        /// </remarks>
        /// <param name="addressDto">Datos de la dirección a crear</param>
        /// <returns>La dirección creada con su ID asignado</returns>
        /// <response code="201">Dirección creada exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<AddressDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertAddress([FromBody] AddressDto addressDto)
        {
            try
            {
                var address = _mapper.Map<Address>(addressDto);
                await _addressService.InsertAsync(address);

                var createdDto = _mapper.Map<AddressDto>(address);
                var response = new ApiResponse<AddressDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Dirección creada exitosamente" } }
                };

                return StatusCode(201, response);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Actualiza la información de una dirección existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de una dirección. Valida todas las reglas de negocio
        /// antes de aplicar los cambios.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/address/dto/mapper/10
        ///{
        ///  "id": 10,
        ///  "customerId": 8,
        ///  "street": "Calle 21 de Mayo #1234",
        ///  "city": "Santa Cruz",
        ///  "department": "Santa Cruz",
        ///  "zone": "Centro",
        ///  "type": "Delivery",
        ///  "isDefault": true,
        ///  "reference": "Edificio El Fuerte",
        ///  "contactName": "Sofia Morales",
        ///  "contactPhone": "71234567",
        ///  "isActive": true
        ///}
        /// </remarks>
        /// <param name="id">Identificador de la dirección a actualizar</param>
        /// <param name="addressDto">Nuevos datos de la dirección</param>
        /// <returns>La dirección actualizada</returns>
        /// <response code="200">Dirección actualizada exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si la dirección no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AddressDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto addressDto)
        {
            try
            {
                if (id != addressDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID de la dirección no coincide" } }
                    });
                }

                var existing = await _addressService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Dirección no encontrada" } }
                    });
                }

                var address = _mapper.Map<Address>(addressDto);
                await _addressService.UpdateAsync(address);

                var updatedDto = _mapper.Map<AddressDto>(address);
                var response = new ApiResponse<AddressDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Dirección actualizada exitosamente" } }
                };

                return Ok(response);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Elimina una dirección del sistema
        /// </summary>
        /// <remarks>
        /// No permite eliminar la única dirección predeterminada activa de un cliente.
        /// Esta es una eliminación física del registro.
        /// Ejemplo de uso:
        /// DELETE /api/v1/address/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador de la dirección a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Dirección eliminada exitosamente</response>
        /// <response code="400">Si no se puede eliminar debido a reglas de negocio</response>
        /// <response code="404">Si la dirección no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var address = await _addressService.GetByIdAsync(id);
                if (address == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Dirección no encontrada" } }
                    });
                }

                await _addressService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }
    }
}