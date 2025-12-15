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
    /// Controller for warehouse management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all warehouse-related operations including CRUD operations,
    /// capacity management, statistics, and shipment tracking through warehouses.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public WarehouseController(
            IWarehouseService warehouseService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _warehouseService = warehouseService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de almacenes con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por nombre, código, ciudad, departamento, tipo y estado.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/dto/mapper?City=La Paz&amp;Type=Regional&amp;IsActive=true&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de almacenes</returns>
        /// <response code="200">Retorna la lista paginada de almacenes</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran almacenes con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<WarehouseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllWarehouses([FromQuery] WarehouseQueryFilter filters)
        {
            try
            {
                var warehouses = await _warehouseService.GetAllAsync(filters);
                var warehousesDto = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = warehouses.Pagination.TotalCount,
                    PageSize = warehouses.Pagination.PageSize,
                    CurrentPage = warehouses.Pagination.CurrentPage,
                    TotalPages = warehouses.Pagination.TotalPages,
                    HasNextPage = warehouses.Pagination.HasNextPage,
                    HasPreviousPage = warehouses.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<WarehouseDto>>(warehousesDto)
                {
                    Pagination = pagination,
                    Messages = warehouses.Messages
                };

                return StatusCode((int)warehouses.StatusCode, response);
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
        /// Obtiene todos los almacenes activos
        /// </summary>
        /// <remarks>
        /// Consulta optimizada que retorna solo los almacenes marcados como activos.
        /// Útil para formularios de selección y operaciones que requieren almacenes operativos.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/dto/mapper/active
        /// </remarks>
        /// <returns>Lista de almacenes activos</returns>
        /// <response code="200">Retorna la lista de almacenes activos</response>
        /// <response code="404">Si no hay almacenes activos</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<WarehouseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveWarehouses()
        {
            try
            {
                var warehouses = await _warehouseService.GetActiveWarehousesAsync();

                if (!warehouses.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "No hay almacenes activos disponibles" } }
                    });
                }

                var warehousesDto = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

                var response = new ApiResponse<IEnumerable<WarehouseDto>>(warehousesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {warehousesDto.Count()} almacenes activos" } }
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
        /// Obtiene almacenes con capacidad disponible mínima
        /// </summary>
        /// <remarks>
        /// Retorna almacenes que tienen al menos la capacidad especificada disponible.
        /// Útil para asignar envíos a almacenes con espacio suficiente.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/dto/available?requiredCapacity=50.5
        /// </remarks>
        /// <param name="requiredCapacity">Capacidad requerida en m³</param>
        /// <returns>Lista de almacenes con capacidad disponible</returns>
        /// <response code="200">Retorna los almacenes disponibles</response>
        /// <response code="400">Si la capacidad requerida es inválida</response>
        /// <response code="404">Si no hay almacenes con capacidad suficiente</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<WarehouseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableWarehouses([FromQuery] double requiredCapacity)
        {
            try
            {
                if (requiredCapacity <= 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "La capacidad requerida debe ser mayor a 0" } }
                    });
                }

                var warehouses = await _warehouseService.GetAvailableWarehousesAsync(requiredCapacity);

                if (!warehouses.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = $"No hay almacenes con al menos {requiredCapacity} m³ disponibles" } }
                    });
                }

                var warehousesDto = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);

                var response = new ApiResponse<IEnumerable<WarehouseDto>>(warehousesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {warehousesDto.Count()} almacenes con capacidad disponible" } }
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
        /// Obtiene un almacén específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del almacén.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador único del almacén</param>
        /// <returns>Información detallada del almacén</returns>
        /// <response code="200">Retorna el almacén solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el almacén no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<WarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetWarehouseById(int id)
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

                var warehouse = await _warehouseService.GetByIdAsync(id);
                var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);

                var response = new ApiResponse<WarehouseDto>(warehouseDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Almacén recuperado exitosamente" } }
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
        /// Obtiene un almacén por su código único
        /// </summary>
        /// <remarks>
        /// Busca un almacén utilizando su código identificador único.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/code/WH-LP-001
        /// </remarks>
        /// <param name="code">Código único del almacén</param>
        /// <returns>Información del almacén</returns>
        /// <response code="200">Retorna el almacén encontrado</response>
        /// <response code="400">Si el código es inválido</response>
        /// <response code="404">Si no existe un almacén con ese código</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<WarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetWarehouseByCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El código es requerido" } }
                    });
                }

                var warehouse = await _warehouseService.GetByCodeAsync(code);
                var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);

                var response = new ApiResponse<WarehouseDto>(warehouseDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Almacén recuperado exitosamente" } }
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
        /// Obtiene estadísticas de operación de un almacén
        /// </summary>
        /// <remarks>
        /// Retorna información estadística del almacén incluyendo:
        /// - Total de envíos procesados
        /// - Envíos actualmente en el almacén
        /// - Envíos despachados
        /// - Porcentaje de ocupación
        /// - Capacidad disponible
        /// - Tiempo promedio de permanencia
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/warehouse/5/statistics
        /// </remarks>
        /// <param name="id">ID del almacén</param>
        /// <returns>Estadísticas del almacén</returns>
        /// <response code="200">Retorna las estadísticas</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el almacén no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<WarehouseStatisticsResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetWarehouseStatistics(int id)
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

                var statistics = await _warehouseService.GetWarehouseStatisticsAsync(id);

                var response = new ApiResponse<WarehouseStatisticsResponse>(statistics)
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
        /// Crea un nuevo almacén
        /// </summary>
        /// <remarks>
        /// Este método valida que el código sea único y que todos los campos cumplan
        /// con las reglas de negocio. Aplica validaciones como:
        /// - Código único (formato: letras, números y guiones)
        /// - Departamento válido (uno de los 9 de Bolivia)
        /// - Capacidad máxima mayor a 0
        /// - Fecha de apertura no puede ser futura
        /// - Coordenadas GPS válidas (opcional)
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/warehouse
        /// {
        ///  "name": "Almacén Central Potosi",
        ///  "code": "WH-PT-001",
        ///  "address": "Av. Argentina #45",
        ///  "city": "Potosi",
        ///  "department": "Potosí",
        ///  "phone": "7-6734512",
        ///  "email": "almacen.potosi@empresa.com",
        ///  "maxCapacityM3": 900,
        ///  "currentCapacityM3": 0,
        ///  "isActive": true,
        ///  "type": "Central",
        ///  "operatingHours": "Lunes a Viernes 8:00-19:00, Sábados 9:00-15:00",
        ///  "managerName": "Daniel Sandoval"
        ///}
        /// </remarks>
        /// <param name="warehouseDto">Datos del almacén a crear</param>
        /// <returns>El almacén creado con su ID asignado</returns>
        /// <response code="201">Almacén creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<WarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertWarehouse([FromBody] WarehouseDto warehouseDto)
        {
            try
            {
                var warehouse = _mapper.Map<Warehouse>(warehouseDto);
                await _warehouseService.InsertAsync(warehouse);

                var createdDto = _mapper.Map<WarehouseDto>(warehouse);
                var response = new ApiResponse<WarehouseDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Almacén creado exitosamente" } }
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
        /// Actualiza la información de un almacén existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un almacén. Valida todas las reglas de negocio
        /// antes de aplicar los cambios.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/warehouse/dto/mapper/2
        ///{
        ///  "id": 2,
        ///  "name": "Almacén Regional Cochabamba",
        ///  "code": "WH-CB-001",
        ///  "address": "Av. Petrolera Km 5, Parque Industrial",
        ///  "city": "Cochabamba",
        ///  "department": "Cochabamba",
        ///  "phone": "7-7237669",
        ///  "email": "almacen.cbba@empresa.com",
        ///  "maxCapacityM3": 1700,
        ///  "currentCapacityM3": 450.5,
        ///  "isActive": true,
        ///  "type": "Regional",
        ///  "operatingHours": "Lunes a Sábado 7:00-19:00",
        ///  "managerName": "Carmen Flores"
        ///}
        /// </remarks>
        /// <param name="id">Identificador del almacén a actualizar</param>
        /// <param name="warehouseDto">Nuevos datos del almacén</param>
        /// <returns>El almacén actualizado</returns>
        /// <response code="200">Almacén actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el almacén no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<WarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDto warehouseDto)
        {
            try
            {
                if (id != warehouseDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del almacén no coincide" } }
                    });
                }

                var existing = await _warehouseService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Almacén no encontrado" } }
                    });
                }

                var warehouse = _mapper.Map<Warehouse>(warehouseDto);
                await _warehouseService.UpdateAsync(warehouse);

                var updatedDto = _mapper.Map<WarehouseDto>(warehouse);
                var response = new ApiResponse<WarehouseDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Almacén actualizado exitosamente" } }
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
        /// Elimina un almacén del sistema
        /// </summary>
        /// <remarks>
        /// No permite eliminar almacenes que tengan envíos activos.
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/warehouse/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador del almacén a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Almacén eliminado exitosamente</response>
        /// <response code="400">Si el almacén tiene envíos activos</response>
        /// <response code="404">Si el almacén no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            try
            {
                var warehouse = await _warehouseService.GetByIdAsync(id);
                if (warehouse == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Almacén no encontrado" } }
                    });
                }
                await _warehouseService.DeleteAsync(id);
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