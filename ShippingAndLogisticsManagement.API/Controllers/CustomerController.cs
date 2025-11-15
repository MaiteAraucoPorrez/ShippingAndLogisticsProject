using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using ShippingAndLogisticsManagement.Infrastructure.Validator;
using System.Net;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Controller for customer management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller allows performing CRUD operations on customers,
    /// including retrieving customer shipment history and validating contact information.
    /// Implements pagination, search filters, and uses Dapper to optimize GET queries.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public CustomerController(
            ICustomerService customerService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _customerService = customerService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de clientes con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por nombre, email, teléfono y estado de envíos activos.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/customer/dto/mapper?Name=Juan&amp;Email=juan@email.com&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de clientes</returns>
        /// <response code="200">Retorna la lista paginada de clientes</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran clientes con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CustomerDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerQueryFilter filters)
        {
            try
            {
                var customers = await _customerService.GetAllAsync(filters);
                var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = customers.Pagination.TotalCount,
                    PageSize = customers.Pagination.PageSize,
                    CurrentPage = customers.Pagination.CurrentPage,
                    TotalPages = customers.Pagination.TotalPages,
                    HasNextPage = customers.Pagination.HasNextPage,
                    HasPreviousPage = customers.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<CustomerDto>>(customersDto)
                {
                    Pagination = pagination,
                    Messages = customers.Messages
                };

                return StatusCode((int)customers.StatusCode, response);
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
        /// Obtiene todos los clientes usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper directamente para obtener clientes
        /// sin aplicar filtros complejos. Ideal para listados simples y rápidos.
        /// Por defecto retorna los últimos 10 registros.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/customer/dto/dapper
        /// </remarks>
        /// <returns>Lista de clientes sin paginación</returns>
        /// <response code="200">Retorna la lista de clientes</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CustomerDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetCustomersDtoDapper()
        {
            try
            {
                var customers = await _customerService.GetAllDapperAsync();
                var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);

                var response = new ApiResponse<IEnumerable<CustomerDto>>(customersDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Clientes recuperados correctamente con Dapper" } }
                };

                return Ok(response);
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
        /// Obtiene un cliente específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del cliente incluyendo datos de contacto.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/customer/5
        /// </remarks>
        /// <param name="id">Identificador único del cliente</param>
        /// <returns>Información detallada del cliente</returns>
        /// <response code="200">Retorna el cliente solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el cliente no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CustomerDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
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

                var customer = await _customerService.GetByIdAsync(id);

                if (customer == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Cliente no encontrado" } }
                    });
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);
                var response = new ApiResponse<CustomerDto>(customerDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Cliente recuperado exitosamente" } }
                };

                return Ok(response);
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
        /// Obtiene un cliente por ID usando Dapper (optimizado para consultas)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// Recomendado para lecturas donde no se requiere modificación posterior.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/customer/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único del cliente</param>
        /// <returns>Información detallada del cliente</returns>
        /// <response code="200">Retorna el cliente solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el cliente no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CustomerDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/{id}")]
        public async Task<IActionResult> GetCustomerByIdDapper(int id)
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

                var customer = await _customerService.GetByIdDapperAsync(id);

                if (customer == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Cliente no encontrado" } }
                    });
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);
                var response = new ApiResponse<CustomerDto>(customerDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Cliente recuperado exitosamente con Dapper" } }
                };

                return Ok(response);
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
        /// Obtiene el historial completo de envíos de un cliente
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna todos los envíos realizados por un cliente,
        /// incluyendo información de paquetes, rutas y estados.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/customer/5/shipments
        /// </remarks>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Historial de envíos del cliente</returns>
        /// <response code="200">Retorna el historial de envíos</response>
        /// <response code="400">Si el ID del cliente es inválido</response>
        /// <response code="404">Si el cliente no existe o no tiene envíos</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CustomerShipmentHistoryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("{customerId}/shipments")]
        public async Task<IActionResult> GetCustomerShipmentHistory(int customerId)
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

                var history = await _customerService.GetCustomerShipmentHistoryAsync(customerId);

                var response = new ApiResponse<IEnumerable<CustomerShipmentHistoryResponse>>(history)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se recuperaron {history.Count()} envíos del cliente" } }
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
        /// Crea un nuevo cliente en el sistema
        /// </summary>
        /// <remarks>
        /// Este método valida que el email sea único y que todos los campos cumplan
        /// con las reglas de negocio. Aplica validaciones como:
        /// - Email único y formato válido
        /// - Teléfono solo dígitos, entre 7 y 20 caracteres
        /// - Nombre entre 3 y 100 caracteres
        /// - No más de 5 clientes con el mismo dominio de email
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/customer
        /// {
        ///   "name": "Juan Carlos Pérez",
        ///   "email": "juan.perez@email.com",
        ///   "phone": "12345678"
        /// }
        /// </remarks>
        /// <param name="customerDto">Datos del cliente a crear</param>
        /// <returns>El cliente creado con su ID asignado</returns>
        /// <response code="201">Cliente creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<CustomerDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertCustomer([FromBody] CustomerDto customerDto)
        {
            try
            {
                var customer = _mapper.Map<Customer>(customerDto);
                await _customerService.InsertAsync(customer);

                var createdDto = _mapper.Map<CustomerDto>(customer);
                var response = new ApiResponse<CustomerDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Cliente creado exitosamente" } }
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
        /// Actualiza la información de un cliente existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un cliente. Valida todas las reglas de negocio
        /// antes de aplicar los cambios, incluyendo unicidad de email.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/customer/5
        /// {
        ///   "id": 5,
        ///   "name": "Juan Carlos Pérez González",
        ///   "email": "juan.perez@email.com",
        ///   "phone": "87654321"
        /// }
        /// </remarks>
        /// <param name="id">Identificador del cliente a actualizar</param>
        /// <param name="customerDto">Nuevos datos del cliente</param>
        /// <returns>El cliente actualizado</returns>
        /// <response code="200">Cliente actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el cliente no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CustomerDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del cliente no coincide" } }
                    });
                }

                var existing = await _customerService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Cliente no encontrado" } }
                    });
                }

                var customer = _mapper.Map<Customer>(customerDto);
                await _customerService.UpdateAsync(customer);

                var updatedDto = _mapper.Map<CustomerDto>(customer);
                var response = new ApiResponse<CustomerDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Cliente actualizado exitosamente" } }
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
            catch (Exception err)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Elimina un cliente del sistema
        /// </summary>
        /// <remarks>
        /// Solo permite eliminar clientes sin envíos activos asociados.
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/customer/5
        /// </remarks>
        /// <param name="id">Identificador del cliente a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Cliente eliminado exitosamente</response>
        /// <response code="400">Si no se puede eliminar debido a envíos activos</response>
        /// <response code="404">Si el cliente no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Cliente no encontrado" } }
                    });
                }

                await _customerService.DeleteAsync(id);
                return NoContent();
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
    }
}