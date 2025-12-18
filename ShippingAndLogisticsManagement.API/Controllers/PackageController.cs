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
    /// Controller for package management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller allows performing CRUD operations on packages,
    /// including assigning packages to shipments and retrieving detailed information.
    /// Implements pagination, search filters, and uses Dapper to optimize GET queries.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public PackageController(
            IPackageService packageService, 
            IMapper mapper, 
            IValidatorService validatorService)
        {
            _packageService = packageService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de paquetes con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por múltiples criterios como ShipmentId, peso mínimo/máximo y descripción.
        /// Retorna los resultados con paginación automática.
        /// Ejemplo de uso:
        /// GET /api/v1/package/dto/mapper?ShipmentId=5&amp;MinWeight=1.0&amp;MaxWeight=10.0&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de paquetes</returns>
        /// <response code="200">Retorna la lista paginada de paquetes</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran paquetes con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PackageDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllPackages(
            [FromQuery] PackageQueryFilter filters)
        {
            try
            {
                var packages = await _packageService.GetAllAsync(filters);
                var packagesDto = _mapper.Map<IEnumerable<PackageDto>>(packages.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = packages.Pagination.TotalCount,
                    PageSize = packages.Pagination.PageSize,
                    CurrentPage = packages.Pagination.CurrentPage,
                    TotalPages = packages.Pagination.TotalPages,
                    HasNextPage = packages.Pagination.HasNextPage,
                    HasPreviousPage = packages.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<PackageDto>>(packagesDto)
                {
                    Pagination = pagination,
                    Messages = packages.Messages
                };

                return StatusCode((int)packages.StatusCode, response);
            }
            catch (Exception err)
            {
                var responsePackage = new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                };
                return StatusCode(500, responsePackage);
            }
        }

        /// <summary>
        /// Obtiene todos los paquetes usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper directamente para obtener paquetes
        /// sin aplicar filtros complejos. Ideal para listados simples y rápidos.
        /// Por defecto retorna los últimos 5 registros.
        /// Ejemplo de uso:
        /// GET /api/v1/package/dto/dapper
        /// </remarks>
        /// <returns>Lista de paquetes sin paginación</returns>
        /// <response code="200">Retorna la lista de paquetes</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PackageDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetPackagesDtoDapper()
        {
            try
            {
                var packages = await _packageService.GetAllDapperAsync();
                var packagesDto = _mapper.Map<IEnumerable<PackageDto>>(packages);

                var response = new ApiResponse<IEnumerable<PackageDto>>(packagesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Paquetes recuperados correctamente con Dapper" } }
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
        /// Obtiene paquetes pesados (mayor a un peso específico)
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna todos los paquetes
        /// que superan un peso mínimo especificado, ordenados por peso descendente.
        /// Útil para planificación de carga y logística de transporte pesado.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/package/dapper/heavy/10.0
        /// </remarks>
        /// <param name="minWeight">Peso mínimo en kilogramos</param>
        /// <example>10.0</example>
        /// <returns>Lista de paquetes pesados ordenados por peso</returns>
        /// <response code="200">Retorna la lista de paquetes pesados</response>
        /// <response code="400">Si el peso mínimo es inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PackageDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/heavy/{minWeight}")]
        public async Task<IActionResult> GetHeavyPackagesAsync(double minWeight)
        {
            try
            {
                if (minWeight <= 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El peso mínimo debe ser mayor a 0" } }
                    });
                }

                var packages = await _packageService.GetHeavyPackagesAsync(minWeight);
                var packagesDto = _mapper.Map<IEnumerable<PackageDto>>(packages);

                var response = new ApiResponse<IEnumerable<PackageDto>>(packagesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {packagesDto.Count()} paquetes con peso mayor a {minWeight} kg" } }
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
        /// Obtiene todos los paquetes con información completa de envío, cliente y ruta
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con JOINs que retorna información detallada de cada paquete
        /// incluyendo datos del envío asociado, cliente y ruta en una sola consulta.
        /// Útil para reportes y vistas consolidadas de paquetes.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/package/details
        /// </remarks>
        /// <returns>Lista completa de paquetes con todos sus detalles relacionados</returns>
        /// <response code="200">Retorna la lista de paquetes con detalles</response>
        /// <response code="404">Si no se encuentran paquetes</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PackageDetailResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("details")]
        public async Task<IActionResult> GetPackageDetailsAsync()
        {
            try
            {
                var packageDetails = await _packageService.GetPackageDetailsAsync();

                if (packageDetails == null || !packageDetails.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "No se encontraron paquetes con detalles" } }
                    });
                }

                var response = new ApiResponse<IEnumerable<PackageDetailResponse>>(packageDetails)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se recuperaron {packageDetails.Count()} paquetes con detalles completos" } }
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
        /// Obtiene un paquete específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del paquete incluyendo datos del envío asociado.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/package/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador único del paquete</param>
        /// <returns>Información detallada del paquete</returns>
        /// <response code="200">Retorna el paquete solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetPackageById(int id)
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

                var package = await _packageService.GetByIdAsync(id);

                if (package == null)
                {
                    return NotFound(new ResponseData
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
            catch (Exception err)
            {
                var errorResponse = new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                };
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Obtiene un paquete por ID usando Dapper (optimizado para consultas)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// Recomendado para lecturas donde no se requiere modificación posterior.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/package/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único del paquete</param>
        /// <example>5</example>
        /// <returns>Información detallada del paquete</returns>
        /// <response code="200">Retorna el paquete solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/{id}")]
        public async Task<IActionResult> GetPackageByIdDapper(int id)
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

                var package = await _packageService.GetByIdDapperAsync(id);

                if (package == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                var packageDto = _mapper.Map<PackageDto>(package);
                var response = new ApiResponse<PackageDto>(packageDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Paquete recuperado exitosamente con Dapper" } }
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
        /// Obtiene el resumen estadístico de paquetes por envío
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna estadísticas agregadas:
        /// - Cantidad total de paquetes
        /// - Peso total acumulado
        /// - Valor total declarado
        /// - Peso promedio por paquete
        /// - Valor promedio por paquete
        /// 
        /// Las agregaciones se calculan directamente en la base de datos
        /// para máxima eficiencia.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/package/summary/5
        /// </remarks>
        /// <param name="shipmentId">ID del envío</param>
        /// <example>5</example>
        /// <returns>Resumen estadístico de paquetes</returns>
        /// <response code="200">Retorna el resumen de paquetes</response>
        /// <response code="400">Si el ID del envío es inválido</response>
        /// <response code="404">Si el envío no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageSummaryResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
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
            catch (KeyNotFoundException err)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = err.Message } }
                });
            }
            catch (ArgumentException err)
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
        /// Crea un nuevo paquete y lo asocia a un envío
        /// </summary>
        /// <remarks>
        /// Este método valida que el envío exista y que esté en un estado válido
        /// para agregar paquetes. Aplica reglas de negocio como:
        /// - Peso mínimo y máximo permitido (0.1-100 kg)
        /// - Estado del envío debe ser "Pending" o "In transit"
        /// - Precio debe ser mayor a 0
        /// - Límite de 50 paquetes por envío
        /// - Descripción mínima de 3 caracteres
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/package
        /// {
        ///   "description": "Laptop Dell XPS 15",
        ///   "weight": 2.5,
        ///   "shipmentId": 5,
        ///   "price": 1500.00
        /// }
        /// </remarks>
        /// <param name="packageDto">Datos del paquete a crear</param>
        /// <returns>El paquete creado con su ID asignado</returns>
        /// <response code="201">Paquete creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertPackageDtoMapper([FromBody] PackageDto packageDto)
        {
            try
            {
                var package = _mapper.Map<Package>(packageDto);
                await _packageService.InsertAsync(package);

                var createdDto = _mapper.Map<PackageDto>(package);
                var response = new ApiResponse<PackageDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Paquete creado exitosamente" } }
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
        /// Actualiza la información de un paquete existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un paquete siempre que el envío asociado
        /// no esté en estado "Delivered". Valida todas las reglas de negocio antes
        /// de aplicar los cambios.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/package/dto/mapper/5
        /// {
        ///   "id": 5,
        ///   "description": "Laptop Dell XPS 15 - Actualizado",
        ///   "weight": 2.8,
        ///   "shipmentId": 5,
        ///   "price": 1600.00
        /// }
        /// </remarks>
        /// <param name="id">Identificador del paquete a actualizar</param>
        /// <example>5</example>
        /// <param name="packageDto">Nuevos datos del paquete</param>
        /// <returns>El paquete actualizado</returns>
        /// <response code="200">Paquete actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<PackageDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdatePackageDtoMapper(int id, 
            [FromBody] PackageDto packageDto)
        {
            try
            {
                if (id != packageDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del paquete no coincide" } }
                    });
                }

                var existing = await _packageService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                existing.Description = packageDto.Description;
                existing.Weight = packageDto.Weight;
                existing.Price = packageDto.Price;
                existing.ShipmentId = packageDto.ShipmentId;

                await _packageService.UpdateAsync(existing);

                var updatedDto = _mapper.Map<PackageDto>(existing);
                var response = new ApiResponse<PackageDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Paquete actualizado exitosamente" } }
                };

                return Ok(response);
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
            catch (Exception err)
            {
                return StatusCode(500, new
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Elimina un paquete del sistema
        /// </summary>
        /// <remarks>
        /// Solo permite eliminar paquetes cuyo envío no esté en estado "Delivered".
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/package/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador del paquete a eliminar</param>
        /// <example>5</example>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Paquete eliminado exitosamente</response>
        /// <response code="400">Si no se puede eliminar debido a reglas de negocio</response>
        /// <response code="404">Si el paquete no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeletePackageDtoMapper(int id)
        {
            try
            {
                var package = await _packageService.GetByIdAsync(id);
                if (package == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Paquete no encontrado" } }
                    });
                }

                await _packageService.DeleteAsync(id);
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