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
    /// Controller for shipment management in the logistics system
    /// </summary>
    /// <remarks>
    /// This controller handles all shipment-related operations including CRUD operations,
    /// filtering, pagination, and integration with customers and routes.
    /// Uses Dapper for optimized read operations and Entity Framework for write operations.
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]    
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public ShipmentController(
            IShipmentService shipmentService, 
            IMapper mapper, 
            IValidatorService validationService)
        {
            _shipmentService = shipmentService;
            _mapper = mapper;
            _validationService = validationService;
        }

        #region CRUD Operations Sin DTOs
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var shipments = await _shipmentService.GetAllAsync();
        //    return Ok(shipments);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var shipment = await _shipmentService.GetByIdAsync(id);
        //    if (shipment == null) return NotFound();
        //    return Ok(shipment);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] Shipment shipment)
        //{
        //    await _shipmentService.InsertAsync(shipment);
        //    return Ok(shipment);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var shipment = await _shipmentService.GetByIdAsync(id);
        //    if (shipment == null) return NotFound();

        //    await _shipmentService.DeleteAsync(shipment);
        //    return Ok();
        //}
        #endregion

        #region CRUD Operations Con DTOs
        //[HttpGet("dto")]
        //public async Task<IActionResult> GetShipmentsDto()
        //{
        //    var shipments = await _shipmentService.GetAllAsync();
        //   var shipmentsDto = shipments.Select(s => new ShipmentDto
        //   {
        //       Id = s.Id,
        //       ShippingDate = s.ShippingDate.ToString("dd-MM-yyyy"),
        //       State = s.State,
        //       CustomerId = s.CustomerId,
        //       RouteId = s.RouteId,
        //       TotalCost = s.TotalCost,
        //       TrackingNumber = s.TrackingNumber
        //   });
        //    return Ok(shipmentsDto);
        //}


        //[HttpGet("dto/{id}")]
        //public async Task<IActionResult> GetShipmentIdDto(int id)
        //{
        //    var shipment = await _shipmentService.GetByIdAsync(id);
        //    var shipmentDto = new ShipmentDto
        //    {
        //        Id = shipment.Id,
        //        ShippingDate = shipment.ShippingDate.ToString("dd-MM-yyyy"),
        //        State = shipment.State,
        //        CustomerId = shipment.CustomerId,
        //        RouteId = shipment.RouteId,
        //        TotalCost = shipment.TotalCost,
        //        TrackingNumber = shipment.TrackingNumber
        //    };

        //    return Ok(shipmentDto);
        //}


        //[HttpPost("dto")]
        //public async Task<IActionResult> InsertPostDto(ShipmentDto shipmentDto)
        //{
        //    var shipment = new Shipment
        //    {
        //        Id = shipmentDto.Id,
        //        ShippingDate = Convert.ToDateTime(shipmentDto.ShippingDate),
        //        State = shipmentDto.State,
        //        CustomerId = shipmentDto.CustomerId,
        //        RouteId = shipmentDto.RouteId,
        //        TotalCost = shipmentDto.TotalCost,
        //        TrackingNumber = shipmentDto.TrackingNumber
        //    };
        //    await _shipmentService.InsertAsync(shipment);
        //    return Ok(shipment);
        //}


        //[HttpPut("dto/{id}")]
        //public async Task<IActionResult> UpdateShipmentDto(int id,
        //    [FromBody] ShipmentDto shipmentDto)
        //{
        //     if (id != shipmentDto.Id)
        //        return BadRequest("El Id del envio no coincide");

        //      var shipment = await _shipmentService.GetByIdAsync(id);
        //      if (shipment == null)
        //          return NotFound("Envio no encontrado");

        //      shipment.Id = shipmentDto.Id;
        //      shipment.ShippingDate = Convert.ToDateTime(shipmentDto.ShippingDate);
        //      shipment.State = shipmentDto.State;
        //      shipment.CustomerId = shipmentDto.CustomerId;
        //      shipment.RouteId = shipmentDto.RouteId;
        //      shipment.TotalCost = shipmentDto.TotalCost;
        //      shipment.TrackingNumber = shipmentDto.TrackingNumber;

        //   await _shipmentService.UpdateAsync(shipment);
        //      return Ok(shipment);
        //}


        //[HttpDelete("dto/{id}")]
        //public async Task<IActionResult> DeleteShipmentDto(int id)
        //{
        //    var shipment = await _shipmentService.GetByIdAsync(id);
        //    if (shipment == null)
        //        return NotFound("Envio no encontrado");

        //    await _shipmentService.DeleteAsync(shipment);
        //    return NoContent();
        //}
        #endregion

        #region ConMapper
        /// <summary>
        /// Retrieves a paginated list of shipment data transfer objects (DTOs) based on the specified query filter
        /// </summary>
        /// <remarks>
        /// This method uses a mapper to convert shipment entities into DTOs, providing a
        /// paginated response. It handles exceptions by returning a 500 status code with error details.
        /// If the parameters aren't sent, it returns all the registers.
        /// 
        /// Sample request:
        ///     GET /api/v1/shipment/dto/mapper?CustomerId=5&amp;State=Pending&amp;PageNumber=1&amp;PageSize=10
        /// </remarks>
        /// <param name="shipmentQueryFilter">The filter criteria used to query shipments. This includes parameters such as page number, page size, and other filtering options.</param>
        /// <example>
        /// {
        ///   "pageNumber": 1,
        ///   "pageSize": 10,
        ///   "customerId": 5,
        ///   "state": "Pending"
        /// }
        /// </example>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with a collection of <see cref="ShipmentDto"/> objects and pagination details. Returns a status code based on the operation result.</returns>
        /// <response code="200">Returns the list of shipments matching the query filter</response>
        /// <response code="204">If no shipments are found matching the query filter</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="404">If no shipments are found matching the query filter</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetShipmentsDtoMapper(
            [FromQuery] ShipmentQueryFilter shipmentQueryFilter)
        {
            try
            {
                var shipments = await _shipmentService.GetAllAsync(shipmentQueryFilter);
                var shipmentsDto = _mapper.Map<IEnumerable<ShipmentDto>>(shipments.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = shipments.Pagination.TotalCount,
                    PageSize = shipments.Pagination.PageSize,
                    CurrentPage = shipments.Pagination.CurrentPage,
                    TotalPages = shipments.Pagination.TotalPages,
                    HasNextPage = shipments.Pagination.HasNextPage,
                    HasPreviousPage = shipments.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<ShipmentDto>>(shipmentsDto)
                {
                    Pagination = pagination,
                    Messages = shipments.Messages
                };

                return StatusCode((int)shipments.StatusCode, response);
            }
            catch (Exception err)
            {
                var responseShipment = new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, responseShipment);
            }
        }

        /// <summary>
        /// Retrieves all shipments using Dapper for optimized database queries
        /// </summary>
        /// <remarks>
        /// This endpoint uses Dapper micro-ORM to execute raw SQL queries for better performance.
        /// Returns shipments without pagination.
        /// 
        /// Sample request:
        ///     GET /api/v1/shipment/dto/dapper
        /// </remarks>
        /// <returns>Collection of shipment DTOs</returns>
        /// <response code="200">Returns the list of all shipments</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetShipmentsDtoMapper()
        {
            var shipments = await _shipmentService.GetAllDapperAsync();
            var shipmentsDto = _mapper.Map<IEnumerable<ShipmentDto>>(shipments);

            var response = new ApiResponse<IEnumerable<ShipmentDto>>(shipmentsDto);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un envío por ID usando Dapper (optimizado para consultas)
        /// </summary>
        /// <remarks>
        /// Este endpoint utiliza Dapper para consultas optimizadas sin tracking.
        /// Recomendado para lecturas donde no se requiere modificación posterior.
        /// 
        /// Sample request:
        ///     GET /api/v1/shipment/dapper/5
        /// </remarks>
        /// <param name="id">Identificador único del envío</param>
        /// <example>5</example>
        /// <returns>Información detallada del envío</returns>
        /// <response code="200">Retorna el envío solicitado</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el envío no existe</response>
        /// <response code="500">Si ocurre un error interno</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ShipmentDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/{id}")]
        public async Task<IActionResult> GetShipmentByIdDapper(int id)
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

                var shipment = await _shipmentService.GetByIdDapperAsync(id);

                if (shipment == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Envío no encontrado" } }
                    });
                }

                var shipmentDto = _mapper.Map<ShipmentDto>(shipment);
                var response = new ApiResponse<ShipmentDto>(shipmentDto)
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Envío recuperado exitosamente con Dapper" } }
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
        /// Retrieves shipments with complete customer and route information using JOIN queries
        /// </summary>
        /// <remarks>
        /// This endpoint executes a complex SQL JOIN to retrieve shipments along with
        /// their associated customer and route details in a single database call.
        /// Optimized for performance using Dapper.
        /// 
        /// Sample request:
        ///     GET /api/v1/shipment/dapper/1
        /// </remarks>
        /// <returns>Collection of shipments with customer and route data</returns>
        /// <response code="200">Returns shipments with complete information</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentCustomerRouteResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dapper/1")]
        public async Task<IActionResult> GetShipmentCustomerRouteAsync()
        {
            var shipments = await _shipmentService.GetShipmentCustomerRouteAsync();


            var response = new ApiResponse<IEnumerable<ShipmentCustomerRouteResponse>>(shipments);

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific shipment by its unique identifier
        /// </summary>
        /// <remarks>
        /// This method validates the ID before querying the database.
        /// Returns detailed information about the shipment.
        /// 
        /// Sample request:
        ///     GET /api/v1/shipment/dto/mapper/5
        /// </remarks>
        /// <param name="id">The unique identifier of the shipment</param>
        /// <example>5</example>
        /// <returns>The shipment with the specified ID</returns>
        /// <response code="200">Returns the requested shipment</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="404">If the shipment is not found</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ShipmentDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetShipmentDtoMapperId(int id)
        {
            #region Validaciones
            var validationRequest = new GetByIdRequest { Id = id };
            var validationResult = await _validationService.ValidateAsync(validationRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseData
                {
                    Messages = validationResult.Errors.Select(e => new Message
                    {
                        Type = "Error",
                        Description = e
                    }).ToArray()
                });
            }
            #endregion
            var shipment = await _shipmentService.GetByIdAsync(id);

            if (shipment == null)
            {
                return NotFound(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No se encontró el envío con ese ID" } }
                });
            }
         
            var shipmentDto = _mapper.Map<ShipmentDto>(shipment);

            var response = new ApiResponse<ShipmentDto>(shipmentDto)
            {
                Messages = new Message[] { new() { Type = "Information", Description = "Envío recuperado exitosamente" } }
            };
            return Ok(response);
        }

        /// <summary>
        /// Creates a new shipment in the system
        /// </summary>
        /// <remarks>
        /// This method validates the shipment data using FluentValidation before insertion.
        /// Business rules applied:
        /// - Customer must exist
        /// - Route must exist and be active
        /// - Tracking number must be unique
        /// - Customer cannot have more than 3 active shipments
        /// - Initial state must be "Pending"
        /// 
        /// Sample request:
        ///     POST /api/v1/shipment/dto/mapper
        ///     {
        ///       "shippingDate": "2025-01-15",
        ///       "state": "Pending",
        ///       "customerId": 5,
        ///       "routeId": 3,
        ///       "totalCost": 150.50,
        ///       "trackingNumber": "TRACK123456"
        ///     }
        /// </remarks>
        /// <param name="shipmentDto">The shipment data to create</param>
        /// <returns>The created shipment with assigned ID</returns>
        /// <response code="200">Shipment created successfully</response>
        /// <response code="400">If validation fails or business rules are violated</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Shipment>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertShipmentDtoMapper(
            [FromBody] ShipmentDto shipmentDto)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var result = await _validationService.ValidateAsync(shipmentDto);
                if (!result.IsValid)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = result.Errors.Select(e => new Message
                        {
                            Type = "Error",
                            Description = e
                        }).ToArray()
                    });

                }
                #endregion

                var shipment = _mapper.Map<Shipment>(shipmentDto);
                shipment.Id = 0;
                await _shipmentService.InsertAsync(shipment);

                var response = new ApiResponse<Shipment>(shipment)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Envío creado exitosamente" } }
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }


        /// <summary>
        /// Updates an existing shipment
        /// </summary>
        /// <remarks>
        /// Business rules:
        /// - Shipment cannot transition to "Delivered" without first being "In transit"
        /// - All validation rules apply
        /// 
        /// Sample request:
        ///     PUT /api/v1/shipment/dto/mapper/5
        ///     {
        ///       "id": 5,
        ///       "shippingDate": "2025-01-15",
        ///       "state": "In transit",
        ///       "customerId": 5,
        ///       "routeId": 3,
        ///       "totalCost": 150.50,
        ///       "trackingNumber": "TRACK123456"
        ///     }
        /// </remarks>
        /// <param name="id">The shipment ID to update</param>
        /// <example>5</example>
        /// <param name="shipmentDto">The updated shipment data</param>
        /// <returns>The updated shipment</returns>
        /// <response code="200">Shipment updated successfully</response>
        /// <response code="400">If validation fails or ID mismatch</response>
        /// <response code="404">If shipment not found</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Shipment>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateShipmentDtoMapper(int id,
            [FromBody] ShipmentDto shipmentDto)
        {
            try
            {
                if (id != shipmentDto.Id)
                {
                    return BadRequest(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Error", Description = "El ID del envío no coincide" } }
                    });
                }

                var existing = await _shipmentService.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Envío no encontrado" } }
                    });
                }

                existing.ShippingDate = DateTime.Parse(shipmentDto.ShippingDate);
                existing.State = shipmentDto.State;
                existing.CustomerId = shipmentDto.CustomerId;
                existing.RouteId = shipmentDto.RouteId;
                existing.TotalCost = shipmentDto.TotalCost;
                existing.TrackingNumber = shipmentDto.TrackingNumber;

                await _shipmentService.UpdateAsync(existing);

                var response = new ApiResponse<Shipment>(existing)
                {
                    Messages = new Message[] { new() { Type = "Success", Description = "Envío actualizado exitosamente" } }
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }

        /// <summary>
        /// Deletes a shipment from the system
        /// </summary>
        /// <remarks>
        /// Physical deletion of the shipment record.
        /// Business rules may prevent deletion based on shipment state.
        /// 
        /// Sample request:
        ///     DELETE /api/v1/shipment/dto/mapper/5
        /// </remarks>
        /// <param name="id">The shipment ID to delete</param>
        /// <example>5</example>
        /// <returns>No content on success</returns>
        /// <response code="204">Shipment deleted successfully</response>
        /// <response code="400">If deletion violates business rules</response>
        /// <response code="404">If shipment not found</response>
        /// <response code="500">If an internal server error occurs</response>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ResponseData))]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteshipmentDtoMapper(int id)
        {
            try
            {
                var shipment = await _shipmentService.GetByIdAsync(id);
                if (shipment == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new Message[] { new() { Type = "Warning", Description = "Envío no encontrado" } }
                    });

                await _shipmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException err)
            {
                return BadRequest(new ResponseData
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } }
                });
            }
        }
        #endregion
    }
}