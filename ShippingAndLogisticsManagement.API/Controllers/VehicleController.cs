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
    /// Controller for vehicle management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all vehicle-related operations including CRUD operations,
    /// filtering, capacity management, maintenance tracking, and vehicle statistics.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public VehicleController(
            IVehicleService vehicleService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _vehicleService = vehicleService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de vehículos con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por placa, marca, modelo, tipo, estado, capacidad y más.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/dto/mapper?Type=Truck&amp;Status=Available&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de vehículos</returns>
        /// <response code="200">Retorna la lista paginada de vehículos</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran vehículos con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<VehicleDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllVehicles([FromQuery] VehicleQueryFilter filters)
        {
            try
            {
                var vehicles = await _vehicleService.GetAllAsync(filters);
                var vehiclesDto = _mapper.Map<IEnumerable<VehicleDto>>(vehicles.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = vehicles.Pagination.TotalCount,
                    PageSize = vehicles.Pagination.PageSize,
                    CurrentPage = vehicles.Pagination.CurrentPage,
                    TotalPages = vehicles.Pagination.TotalPages,
                    HasNextPage = vehicles.Pagination.HasNextPage,
                    HasPreviousPage = vehicles.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<VehicleDto>>(vehiclesDto)
                {
                    Pagination = pagination,
                    Messages = vehicles.Messages
                };

                return StatusCode((int)vehicles.StatusCode, response);
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
        /// Obtiene todos los vehículos usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper directamente para obtener vehículos
        /// sin aplicar filtros complejos. Ideal para listados simples y rápidos.
        /// Por defecto retorna los últimos 10 registros.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/dto/dapper
        /// </remarks>
        /// <returns>Lista de vehículos sin paginación</returns>
        /// <response code="200">Retorna la lista de vehículos</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<VehicleDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetVehiclesDapper()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllDapperAsync();
                var vehiclesDto = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);

                var response = new ApiResponse<IEnumerable<VehicleDto>>(vehiclesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Vehículos recuperados correctamente con Dapper" } }
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
        /// Obtiene vehículos disponibles para asignación
        /// </summary>
        /// <remarks>
        /// Retorna vehículos que están activos, disponibles, sin conductor asignado
        /// y en condiciones operativas. Ordenados por tipo y capacidad.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/dto/available
        /// </remarks>
        /// <returns>Lista de vehículos disponibles</returns>
        /// <response code="200">Retorna los vehículos disponibles</response>
        /// <response code="404">Si no hay vehículos disponibles</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<VehicleDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/available")]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAvailableVehiclesAsync();

                if (!vehicles.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "No hay vehículos disponibles en este momento" } }
                    });
                }

                var vehiclesDto = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);

                var response = new ApiResponse<IEnumerable<VehicleDto>>(vehiclesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {vehiclesDto.Count()} vehículos disponibles" } }
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
        /// Obtiene vehículos con capacidad suficiente
        /// </summary>
        /// <remarks>
        /// Retorna vehículos disponibles que tienen al menos la capacidad especificada.
        /// Útil para asignar envíos que requieren capacidades específicas.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/by-capacity?requiredWeight=1500&amp;requiredVolume=10
        /// </remarks>
        /// <param name="requiredWeight">Peso requerido en kg</param>
        /// <param name="requiredVolume">Volumen requerido en m³</param>
        /// <returns>Lista de vehículos con capacidad suficiente</returns>
        /// <response code="200">Retorna los vehículos</response>
        /// <response code="400">Si los parámetros son inválidos</response>
        /// <response code="404">Si no hay vehículos con capacidad suficiente</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<VehicleDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("by-capacity")]
        public async Task<IActionResult> GetVehiclesByCapacity(
            [FromQuery] double requiredWeight,
            [FromQuery] double requiredVolume)
        {
            try
            {
                if (requiredWeight < 0 || requiredVolume < 0)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El peso y volumen deben ser mayores o iguales a 0" } }
                    });
                }

                var vehicles = await _vehicleService.GetByCapacityAsync(requiredWeight, requiredVolume);

                if (!vehicles.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = $"No hay vehículos con capacidad suficiente ({requiredWeight} kg, {requiredVolume} m³)" } }
                    });
                }

                var vehiclesDto = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);

                var response = new ApiResponse<IEnumerable<VehicleDto>>(vehiclesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {vehiclesDto.Count()} vehículos con capacidad suficiente" } }
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
        /// Obtiene un vehículo específico por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa del vehículo.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador único del vehículo</param>
        /// <returns>Información detallada del vehículo</returns>
        /// <response code="200">Retorna el vehículo solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el vehículo no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<VehicleDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
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

                var vehicle = await _vehicleService.GetByIdAsync(id);
                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);

                var response = new ApiResponse<VehicleDto>(vehicleDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Vehículo recuperado exitosamente" } }
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
        /// Obtiene un vehículo por ID usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/dto/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único del vehículo</param>
        /// <returns>Información detallada del vehículo</returns>
        /// <response code="200">Retorna el vehículo solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el vehículo no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<VehicleDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper/{id}")]
        public async Task<IActionResult> GetVehicleByIdDapper(int id)
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

                var vehicle = await _vehicleService.GetByIdDapperAsync(id);
                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);

                var response = new ApiResponse<VehicleDto>(vehicleDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Vehículo recuperado exitosamente con Dapper" } }
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
        /// Obtiene estadísticas de un vehículo
        /// </summary>
        /// <remarks>
        /// Retorna información estadística del vehículo incluyendo:
        /// - Total de envíos transportados
        /// - Envíos actualmente en tránsito
        /// - Envíos completados
        /// - Porcentaje de ocupación
        /// - Capacidad disponible
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/vehicle/5/statistics
        /// </remarks>
        /// <param name="id">ID del vehículo</param>
        /// <returns>Estadísticas del vehículo</returns>
        /// <response code="200">Retorna las estadísticas</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el vehículo no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<VehicleStatisticsResponse>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetVehicleStatistics(int id)
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

                var statistics = await _vehicleService.GetVehicleStatisticsAsync(id);

                var response = new ApiResponse<VehicleStatisticsResponse>(statistics)
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
        /// Crea un nuevo vehículo
        /// </summary>
        /// <remarks>
        /// Este método valida que la placa sea única y que todos los campos cumplan
        /// con las reglas de negocio. Aplica validaciones como:
        /// - Placa única (formato: letras, números y guiones)
        /// - Capacidad máxima mayor a 0
        /// - Año válido (1900 hasta año actual + 1)
        /// - VIN único de 17 caracteres (opcional)
        /// - Validación de almacén y conductor si se asignan
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/vehicle
        ///{
        ///  "plateNumber": "1111-ABC",
        ///  "type": 4,
        ///  "maxWeightCapacityKg": 3500.00,
        ///  "maxVolumeCapacityM3": 15.5,
        ///  "currentWeightKg": 0,
        ///  "currentVolumeM3": 0,
        ///  "status": 1,
        ///  "vin": "7HGBH41JXMN109876",
        ///  "isActive": true,
        ///  "baseWarehouseId": 2,
        ///  "assignedDriverId": null
        //}
        /// </remarks>
        /// <param name="vehicleDto">Datos del vehículo a crear</param>
        /// <returns>El vehículo creado con su ID asignado</returns>
        /// <response code="201">Vehículo creado exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<VehicleDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertVehicle([FromBody] VehicleDto vehicleDto)
        {
            try
            {
                var vehicle = _mapper.Map<Vehicle>(vehicleDto);
                await _vehicleService.InsertAsync(vehicle);

                var createdDto = _mapper.Map<VehicleDto>(vehicle);
                var response = new ApiResponse<VehicleDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Vehículo creado exitosamente" } }
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
        /// Actualiza la información de un vehículo existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de un vehículo. Valida todas las reglas de negocio
        /// antes de aplicar los cambios, incluyendo unicidad de placa.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/vehicle/dto/mapper/5
        ///{
        ///  "id": 5,
        ///  "plateNumber": "5678-EFG",
        ///  "type": 2,
        ///  "maxWeightCapacityKg": 1500.00,
        ///  "maxVolumeCapacityM3": 10.5,
        ///  "currentWeightKg": 590.00,
        ///  "currentVolumeM3": 5.8,
        ///  "status": 1,
        ///  "vin": "5HGBH41JXMN109190",
        ///  "isActive": true,
        ///  "baseWarehouseId": 5,
        ///  "assignedDriverId": 5
        ///}
        /// </remarks>
        /// <param name="id">Identificador del vehículo a actualizar</param>
        /// <param name="vehicleDto">Nuevos datos del vehículo</param>
        /// <returns>El vehículo actualizado</returns>
        /// <response code="200">Vehículo actualizado exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si el vehículo no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<VehicleDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDto vehicleDto)
        {
            try
            {
                if (id != vehicleDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del vehículo no coincide" } }
                    });
                }

                var existing = await _vehicleService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Vehículo no encontrado" } }
                    });
                }

                var vehicle = _mapper.Map<Vehicle>(vehicleDto);
                await _vehicleService.UpdateAsync(vehicle);

                var updatedDto = _mapper.Map<VehicleDto>(vehicle);
                var response = new ApiResponse<VehicleDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Vehículo actualizado exitosamente" } }
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
        /// Elimina un vehículo del sistema
        /// </summary>
        /// <remarks>
        /// No permite eliminar vehículos que tengan conductor asignado o que estén en tránsito.
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/vehicle/dto/mapper/5
        /// </remarks>
        /// <param name="id">Identificador del vehículo a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Vehículo eliminado exitosamente</response>
        /// <response code="400">Si el vehículo tiene conductor asignado o está en tránsito</response>
        /// <response code="404">Si el vehículo no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetByIdAsync(id);
                if (vehicle == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Vehículo no encontrado" } }
                    });
                }

                await _vehicleService.DeleteAsync(id);
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