using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Net;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Controller for package management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller allows performing CRUD operations on packages,
    /// including assigning packages to shipments and retrieving detailed information.
    /// Implements pagination, search filters, and uses Dapper to optimize GET queries.
    /// </remarks>
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IMapper _mapper;

        public PackageController(IPackageService packageService, IMapper mapper)
        {
            _packageService = packageService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista paginada de paquetes con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por múltiples criterios como ShipmentId, peso mínimo/máximo y descripción.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// 
        ///     GET /api/v1/package?pageNumber=1&amp;pageSize=10&amp;shipmentId=5
        /// 
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de paquetes</returns>
        /// <response code="200">Retorna la lista paginada de paquetes</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran paquetes con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PackageDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllPackages([FromQuery] PackageQueryFilter filters)
        {
            try
            {
                var result = await _packageService.GetAllAsync(filters);
                var packageItems = result.Pagination.Cast<Package>();
                var packagesDto = _mapper.Map<IEnumerable<PackageDto>>(packageItems);



                var pagination = new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<PackageDto>>(packagesDto)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                };
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Obtiene un paquete específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del paquete incluyendo datos del envío asociado.
        /// </remarks>
        /// <param name="id">Identificador único del paquete</param>
        /// <returns>Información detallada del paquete</returns>
        /// <response code="200">Retorna el paquete solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID debe ser mayor a 0" } }
                    });
                }

                var package = await _packageService.GetByIdAsync(id);

                if (package == null)
                {
                    return NotFound(new
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                var packageDto = _mapper.Map<PackageDto>(package);
                var response = new ApiResponse<PackageDto>(packageDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Paquete recuperado exitosamente" } }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                };
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Obtiene el resumen de paquetes por envío
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna estadísticas agregadas:
        /// cantidad total de paquetes, peso total y valor total por envío.
        /// </remarks>
        /// <param name="shipmentId">ID del envío</param>
        /// <returns>Resumen estadístico de paquetes</returns>
        /// <response code="200">Retorna el resumen de paquetes</response>
        /// <response code="404">Si el envío no tiene paquetes</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("summary/{shipmentId}")]
        public async Task<IActionResult> GetPackageSummary(int shipmentId)
        {
            try
            {
                var summary = await _packageService.GetPackageSummaryAsync(shipmentId);

                var response = new ApiResponse<PackageSummaryResponse>(summary)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Resumen recuperado exitosamente" } }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
        }


        /// <summary>
        /// Crea un nuevo paquete y lo asocia a un envío
        /// </summary>
        /// <remarks>
        /// Este método valida que el envío exista y que esté en un estado válido
        /// para agregar paquetes. Aplica reglas de negocio como:
        /// - Peso mínimo y máximo permitido
        /// - Estado del envío debe ser "Pending" o "In transit"
        /// - Precio debe ser mayor a 0
        /// </remarks>
        /// <param name="packageDto">Datos del paquete a crear</param>
        /// <returns>El paquete creado con su ID asignado</returns>
        /// <response code="201">Paquete creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] PackageDto packageDto)
        {
            try
            {
                var package = _mapper.Map<Package>(packageDto);
                //package.Id = 0;

                await _packageService.InsertAsync(package);

                var createdDto = _mapper.Map<PackageDto>(package);
                var response = new ApiResponse<PackageDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Paquete creado exitosamente" } }
                };

                return StatusCode(201, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Actualiza la información de un paquete existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un paquete siempre que el envío asociado
        /// no esté en estado "Delivered". Valida todas las reglas de negocio antes
        /// de aplicar los cambios.
        /// </remarks>
        /// <param name="id">Identificador del paquete a actualizar</param>
        /// <param name="packageDto">Nuevos datos del paquete</param>
        /// <returns>El paquete actualizado</returns>
        /// <response code="200">Paquete actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] PackageDto packageDto)
        {
            try
            {
                if (id != packageDto.Id)
                {
                    return BadRequest(new
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del paquete no coincide" } }
                    });
                }

                var existing = await _packageService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                var package = _mapper.Map<Package>(packageDto);
                await _packageService.UpdateAsync(package);

                var updatedDto = _mapper.Map<PackageDto>(package);
                var response = new ApiResponse<PackageDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Paquete actualizado exitosamente" } }
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Elimina un paquete del sistema
        /// </summary>
        /// <remarks>
        /// Solo permite eliminar paquetes cuyo envío no esté en estado "Delivered".
        /// Esta es una eliminación física del registro.
        /// </remarks>
        /// <param name="id">Identificador del paquete a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Paquete eliminado exitosamente</response>
        /// <response code="400">Si no se puede eliminar debido a reglas de negocio</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                var package = await _packageService.GetByIdAsync(id);
                if (package == null)
                {
                    return NotFound(new
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                await _packageService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = ex.Message } }
                });
            }
        }
    }
}