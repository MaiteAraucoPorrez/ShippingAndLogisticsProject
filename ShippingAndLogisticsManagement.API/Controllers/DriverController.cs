using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Core.Services;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using ShippingAndLogisticsManagement.Infrastructure.Validator;
using System.Net;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Controller for driver management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all driver-related operations including CRUD operations,
    /// filtering, license management, vehicle assignments, and driver statistics.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public DriverController(
            IDriverService driverService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _driverService = driverService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de conductores con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por nombre, licencia, categoría, estado, experiencia y calificación.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/dto/mapper?FullName=Juan&amp;Status=Available&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de conductores</returns>
        /// <response code="200">Retorna la lista paginada de conductores</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran conductores con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<DriverDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllDrivers([FromQuery] DriverQueryFilter filters)
        {
            try
            {
                var drivers = await _driverService.GetAllAsync(filters);
                var driversDto = _mapper.Map<IEnumerable<DriverDto>>(drivers.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = drivers.Pagination.TotalCount,
                    PageSize = drivers.Pagination.PageSize,
                    CurrentPage = drivers.Pagination.CurrentPage,
                    TotalPages = drivers.Pagination.TotalPages,
                    HasNextPage = drivers.Pagination.HasNextPage,
                    HasPreviousPage = drivers.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<DriverDto>>(driversDto)
                {
                    Pagination = pagination,
                    Messages = drivers.Messages
                };

                return StatusCode((int)drivers.StatusCode, response);
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
        /// Obtiene todos los conductores usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper directamente para obtener conductores
        /// sin aplicar filtros complejos. Ideal para listados simples y rápidos.
        /// Por defecto retorna los últimos 10 registros.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/dto/dapper
        /// </remarks>
        /// <returns>Lista de conductores sin paginación</returns>
        /// <response code="200">Retorna la lista de conductores</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<DriverDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetDriversDapper()
        {
            try
            {
                var drivers = await _driverService.GetAllDapperAsync();
                var driversDto = _mapper.Map<IEnumerable<DriverDto>>(drivers);

                var response = new ApiResponse<IEnumerable<DriverDto>>(driversDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Conductores recuperados correctamente con Dapper" } }
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
        /// Obtiene conductores disponibles para asignación
        /// </summary>
        /// <remarks>
        /// Retorna conductores que están activos, disponibles, sin vehículo asignado
        /// y con licencia vigente. Ordenados por experiencia y calificación.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/dto/available
        /// </remarks>
        /// <returns>Lista de conductores disponibles</returns>
        /// <response code="200">Retorna los conductores disponibles</response>
        /// <response code="404">Si no hay conductores disponibles</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<DriverDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/available")]
        public async Task<IActionResult> GetAvailableDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAvailableDriversAsync();

                if (!drivers.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "No hay conductores disponibles en este momento" } }
                    });
                }

                var driversDto = _mapper.Map<IEnumerable<DriverDto>>(drivers);

                var response = new ApiResponse<IEnumerable<DriverDto>>(driversDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {driversDto.Count()} conductores disponibles" } }
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
        /// Obtiene un conductor específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del conductor.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador único del conductor</param>
        /// <returns>Información detallada del conductor</returns>
        /// <response code="200">Retorna el conductor solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el conductor no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<DriverDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetDriverById(int id)
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

                var driver = await _driverService.GetByIdAsync(id);
                var driverDto = _mapper.Map<DriverDto>(driver);

                var response = new ApiResponse<DriverDto>(driverDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Conductor recuperado exitosamente" } }
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
        /// Obtiene un conductor por ID usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/dto/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único del conductor</param>
        /// <returns>Información detallada del conductor</returns>
        /// <response code="200">Retorna el conductor solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el conductor no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<DriverDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper/{id}")]
        public async Task<IActionResult> GetDriverByIdDapper(int id)
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

                var driver = await _driverService.GetByIdDapperAsync(id);
                var driverDto = _mapper.Map<DriverDto>(driver);

                var response = new ApiResponse<DriverDto>(driverDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Conductor recuperado exitosamente con Dapper" } }
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
        /// Obtiene estadísticas de un conductor
        /// </summary>
        /// <remarks>
        /// Retorna información estadística del conductor incluyendo:
        /// - Total de entregas completadas
        /// - Entregas a tiempo vs tardías
        /// - Porcentaje de entregas puntuales
        /// - Calificación promedio
        /// - Años de experiencia
        /// - Estado de licencia (días hasta vencimiento)
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/driver/5/statistics
        /// </remarks>
        /// <param name="id">ID del conductor</param>
        /// <returns>Estadísticas del conductor</returns>
        /// <response code="200">Retorna las estadísticas</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el conductor no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<DriverStatisticsResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetDriverStatistics(int id)
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

                var statistics = await _driverService.GetDriverStatisticsAsync(id);

                var response = new ApiResponse<DriverStatisticsResponse>(statistics)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Estadísticas recuperadas exitosamente" } }
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
        /// Crea un nuevo conductor
        /// </summary>
        /// <remarks>
        /// Este método valida que la licencia y documento sean únicos y que todos los campos 
        /// cumplan con las reglas de negocio. Aplica validaciones como:
        /// - Documento de identidad único
        /// - Número de licencia único
        /// - Licencia vigente (no vencida)
        /// - Mayor de 18 años
        /// - Email válido
        /// - Teléfono entre 7 y 20 caracteres
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/driver
        ///{
        ///  "fullName": "Juan Carlos Pérez González",
        ///  "licenseNumber": "LIC-2024-001234",
        ///  "phone": "71234567",
        ///  "email": "juan.perez@empresa.com",
        ///  "address": "Av. 6 de Agosto #1234, Zona Sopocachi",
        ///  "city": "La Paz",
        ///  "dateOfBirth": "1985-05-20",
        ///  "isActive": true,
        ///  "status": 1,
        ///  "totalDeliveries": 0
        ///}
        /// </remarks>
        /// <param name="driverDto">Datos del conductor a crear</param>
        /// <returns>El conductor creado con su ID asignado</returns>
        /// <response code="201">Conductor creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<DriverDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertDriver([FromBody] DriverDto driverDto)
        {
            try
            {
                var driver = _mapper.Map<Driver>(driverDto);
                await _driverService.InsertAsync(driver);

                var createdDto = _mapper.Map<DriverDto>(driver);
                var response = new ApiResponse<DriverDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Conductor creado exitosamente" } }
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
        /// Actualiza la información de un conductor existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un conductor. Valida todas las reglas de negocio
        /// antes de aplicar los cambios, incluyendo unicidad de licencia y documento.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/driver/dto/mapper/5
        ///{
        ///  "id": 5,
        ///  "fullName": "Juan Carlos Pérez González (ACTUALIZADO)",
        ///  "licenseNumber": "LIC-2024-001234",
        ///  "phone": "71234567",
        ///  "email": "juan.perez@empresa.com",
        ///  "address": "Nueva dirección",
        ///  "city": "La Paz",
        ///  "dateOfBirth": "1985-05-20",
        ///  "isActive": true,
        ///  "status": 2,
        ///  "currentVehicleId": 3,
        ///  "totalDeliveries": 150
        ///}
        /// </remarks>
        /// <param name="id">Identificador del conductor a actualizar</param>
        /// <param name="driverDto">Nuevos datos del conductor</param>
        /// <returns>El conductor actualizado</returns>
        /// <response code="200">Conductor actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el conductor no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<DriverDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] DriverDto driverDto)
        {
            try
            {
                if (id != driverDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del conductor no coincide" } }
                    });
                }

                var existing = await _driverService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Conductor no encontrado" } }
                    });
                }

                var driver = _mapper.Map<Driver>(driverDto);
                await _driverService.UpdateAsync(driver);

                var updatedDto = _mapper.Map<DriverDto>(driver);
                var response = new ApiResponse<DriverDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Conductor actualizado exitosamente" } }
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
        /// Elimina un conductor del sistema
        /// </summary>
        /// <remarks>
        /// No permite eliminar conductores con vehículos asignados.
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/driver/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador del conductor a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Conductor eliminado exitosamente</response>
        /// <response code="400">Si el conductor tiene vehículo asignado</response>
        /// <response code="404">Si el conductor no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            try
            {
                var driver = await _driverService.GetByIdAsync(id);
                if (driver == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Conductor no encontrado" } }
                    });
                }

                await _driverService.DeleteAsync(id);
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