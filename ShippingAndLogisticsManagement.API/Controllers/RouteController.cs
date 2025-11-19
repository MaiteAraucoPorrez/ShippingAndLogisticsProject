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
    /// Controller for route management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all route-related operations including CRUD operations,
    /// filtering, pagination, and specialized queries for route analytics.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public RouteController(
            IRouteService routeService,
            IMapper mapper,
            IValidatorService validatorService)
        {
            _routeService = routeService;
            _mapper = mapper;
            _validationService = validatorService;
        }

        /// <summary>
        /// Obtiene una lista paginada de rutas con filtros opcionales
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para realizar consultas optimizadas y permite
        /// filtrar por origen, destino, rango de distancia, costo y estado activo/inactivo.
        /// Retorna los resultados con paginación automática.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/dto/mapper?Origin=Cochabamba&amp;Destination=Santa Cruz&amp;IsActive=true&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda incluyendo paginación</param>
        /// <returns>Lista paginada de rutas</returns>
        /// <response code="200">Retorna la lista paginada de rutas</response>
        /// <response code="400">Si los parámetros de filtro son inválidos</response>
        /// <response code="404">Si no se encuentran rutas con los filtros especificados</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<RouteDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetAllRoutes([FromQuery] RouteQueryFilter filters)
        {
            try
            {
                var routes = await _routeService.GetAllAsync(filters);
                var routesDto = _mapper.Map<IEnumerable<RouteDto>>(routes.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = routes.Pagination.TotalCount,
                    PageSize = routes.Pagination.PageSize,
                    CurrentPage = routes.Pagination.CurrentPage,
                    TotalPages = routes.Pagination.TotalPages,
                    HasNextPage = routes.Pagination.HasNextPage,
                    HasPreviousPage = routes.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<RouteDto>>(routesDto)
                {
                    Pagination = pagination,
                    Messages = routes.Messages
                };

                return StatusCode((int)routes.StatusCode, response);
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
        /// Obtiene todas las rutas usando Dapper (optimizado)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper directamente para obtener rutas
        /// sin aplicar filtros complejos. Ideal para listados simples y rápidos.
        /// Por defecto retorna los últimos 10 registros.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/dto/dapper
        /// </remarks>
        /// <returns>Lista de rutas sin paginación</returns>
        /// <response code="200">Retorna la lista de rutas</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<RouteDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetRoutesDtoDapper()
        {
            try
            {
                var routes = await _routeService.GetAllDapperAsync();
                var routesDto = _mapper.Map<IEnumerable<RouteDto>>(routes);

                var response = new ApiResponse<IEnumerable<RouteDto>>(routesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Rutas recuperadas correctamente con Dapper" } }
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
        /// Obtiene una ruta específica por su identificador
        /// </summary>
        /// <remarks>
        /// Este método utiliza Dapper para optimizar la consulta y retorna
        /// la información completa de la ruta incluyendo costo por kilómetro.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/5
        /// </remarks>
        /// <param name="id">Identificador único de la ruta</param>
        /// <returns>Información detallada de la ruta</returns>
        /// <response code="200">Retorna la ruta solicitada</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si la ruta no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<RouteDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetRouteById(int id)
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

                var route = await _routeService.GetByIdAsync(id);

                if (route == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Ruta no encontrada" } }
                    });
                }

                var routeDto = _mapper.Map<RouteDto>(route);
                var response = new ApiResponse<RouteDto>(routeDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Ruta recuperada exitosamente" } }
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
        /// Obtiene una ruta por ID usando Dapper (optimizado para consultas)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// Recomendado para lecturas donde no se requiere modificación posterior.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único de la ruta</param>
        /// <returns>Información detallada de la ruta</returns>
        /// <response code="200">Retorna la ruta solicitada</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si la ruta no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<RouteDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/{id}")]
        public async Task<IActionResult> GetRouteByIdDapper(int id)
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

                var route = await _routeService.GetByIdDapperAsync(id);

                if (route == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Ruta no encontrada" } }
                    });
                }

                var routeDto = _mapper.Map<RouteDto>(route);
                var response = new ApiResponse<RouteDto>(routeDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Ruta recuperada exitosamente con Dapper" } }
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
        /// Obtiene el ranking de rutas más utilizadas
        /// </summary>
        /// <remarks>
        /// Consulta optimizada con Dapper que retorna las rutas ordenadas por
        /// la cantidad de envíos que han procesado, útil para análisis de demanda.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/ranking?limit=10
        /// </remarks>
        /// <param name="limit">Cantidad de rutas a retornar (por defecto 10)</param>
        /// <returns>Ranking de rutas más utilizadas</returns>
        /// <response code="200">Retorna el ranking de rutas</response>
        /// <response code="400">Si el límite es inválido</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<RouteRankingResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("ranking")]
        public async Task<IActionResult> GetRouteRanking([FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El límite debe estar entre 1 y 100" } }
                    });
                }

                var ranking = await _routeService.GetMostUsedRoutesAsync(limit);

                var response = new ApiResponse<IEnumerable<RouteRankingResponse>>(ranking)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se recuperaron las {ranking.Count()} rutas más utilizadas" } }
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
        /// Obtiene solo las rutas activas disponibles para nuevos envíos
        /// </summary>
        /// <remarks>
        /// Filtra y retorna únicamente las rutas marcadas como activas (IsActive = true).
        /// Útil para formularios de selección de rutas en la creación de envíos.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/route/active
        /// </remarks>
        /// <returns>Lista de rutas activas</returns>
        /// <response code="200">Retorna las rutas activas</response>
        /// <response code="404">Si no hay rutas activas disponibles</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<RouteDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveRoutes()
        {
            try
            {
                var routes = await _routeService.GetActiveRoutesAsync();

                if (!routes.Any())
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "No hay rutas activas disponibles" } }
                    });
                }

                var routesDto = _mapper.Map<IEnumerable<RouteDto>>(routes);
                var response = new ApiResponse<IEnumerable<RouteDto>>(routesDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = $"Se encontraron {routesDto.Count()} rutas activas" } }
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
        /// Crea una nueva ruta en el sistema
        /// </summary>
        /// <remarks>
        /// Este método valida que la ruta no exista previamente y que cumpla
        /// con todas las reglas de negocio. Aplica validaciones como:
        /// - Distancia mayor a 0 km
        /// - Costo base mayor o igual a 0
        /// - Origen y destino diferentes
        /// - No existir ruta duplicada (mismo origen-destino)
        /// - Nombres de ciudades entre 3 y 100 caracteres
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/route
        /// {
        ///   "origin": "Cochabamba",
        ///   "destination": "Santa Cruz",
        ///   "distanceKm": 502.5,
        ///   "baseCost": 50.00,
        ///   "isActive": true
        /// }
        /// </remarks>
        /// <param name="routeDto">Datos de la ruta a crear</param>
        /// <returns>La ruta creada con su ID asignado</returns>
        /// <response code="201">Ruta creada exitosamente</response>
        /// <response code="400">Si los datos son inválidos o no cumplen las reglas de negocio</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<RouteDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost]
        public async Task<IActionResult> InsertRoute([FromBody] RouteDto routeDto)
        {
            try
            {
                var route = _mapper.Map<Core.Entities.Route>(routeDto);
                await _routeService.InsertAsync(route);

                var createdDto = _mapper.Map<RouteDto>(route);
                var response = new ApiResponse<RouteDto>(createdDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Ruta creada exitosamente" } }
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
        /// Actualiza la información de una ruta existente
        /// </summary>
        /// <remarks>
        /// Permite modificar los datos de una ruta. Valida todas las reglas de negocio
        /// antes de aplicar los cambios. No se puede cambiar origen/destino si hay envíos asociados.
        /// 
        /// Ejemplo de solicitud:
        /// PUT /api/v1/route/5
        /// {
        ///   "id": 5,
        ///   "origin": "Cochabamba",
        ///   "destination": "Santa Cruz",
        ///   "distanceKm": 505.0,
        ///   "baseCost": 55.00,
        ///   "isActive": true
        /// }
        /// </remarks>
        /// <param name="id">Identificador de la ruta a actualizar</param>
        /// <param name="routeDto">Nuevos datos de la ruta</param>
        /// <returns>La ruta actualizada</returns>
        /// <response code="200">Ruta actualizada exitosamente</response>
        /// <response code="400">Si el ID no coincide o los datos son inválidos</response>
        /// <response code="404">Si la ruta no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<RouteDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteDto routeDto)
        {
            try
            {
                if (id != routeDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID de la ruta no coincide" } }
                    });
                }

                var existing = await _routeService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Ruta no encontrada" } }
                    });
                }

                existing.Origin = routeDto.Origin;
                existing.Destination = routeDto.Destination;
                existing.DistanceKm = routeDto.DistanceKm;
                existing.BaseCost = routeDto.BaseCost;
                existing.IsActive = routeDto.IsActive;

                await _routeService.UpdateAsync(existing);

                var updatedDto = _mapper.Map<RouteDto>(existing);
                var response = new ApiResponse<RouteDto>(updatedDto)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Ruta actualizada exitosamente" } }
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
        /// Elimina una ruta del sistema
        /// </summary>
        /// <remarks>
        /// Solo permite eliminar rutas sin envíos asociados.
        /// Esta es una eliminación física del registro.
        /// 
        /// Ejemplo de uso:
        /// DELETE /api/v1/route/5
        /// </remarks>
        /// <param name="id">Identificador de la ruta a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="204">Ruta eliminada exitosamente</response>
        /// <response code="400">Si no se puede eliminar debido a envíos asociados</response>
        /// <response code="404">Si la ruta no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var route = await _routeService.GetByIdAsync(id);
                if (route == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Ruta no encontrada" } }
                    });
                }

                await _routeService.DeleteAsync(id);
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