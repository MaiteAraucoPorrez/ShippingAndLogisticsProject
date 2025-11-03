using AutoMapper;
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
    [Produces("application/json")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validationService;

        public ShipmentController(IShipmentService shipmentService, IMapper mapper, IValidatorService validationService)
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
        /// Retrieves a paginated list of shipment data transfer objects (DTOs) based on the specified query filter.
        /// </summary>
        /// <remarks>
        /// This method uses a mapper to convert shipment entities into DTOs, providing a
        /// paginated response. It handles exceptions by returning a 500 status code with error details.
        /// If the parameters aren't send, it returns all the registers.
        /// </remarks>
        /// <param name="shipmentQueryFilter">The filter criteria used to query shipments. This includes parameters such as page number, page size, and
        /// other filtering options.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with a collection of <see
        /// cref="ShipmentDto"/> objects and pagination details. Returns a status code based on the operation result.</returns>
        /// <response code="200">Returns the list of shipments matching the query filter.</response>
        /// <response code="204">If no shipments are found matching the query filter.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="401">If the user is unauthorized to access this resource.</response>
        /// <response code="404">If no shipments are found matching the query filter.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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

        [HttpGet("dto/dapper")]
        public async Task<IActionResult> GetShipmentsDtoMapper()
        {
            var shipments = await _shipmentService.GetAllDapperAsync();
            var shipmentsDto = _mapper.Map<IEnumerable<ShipmentDto>>(shipments);

            var response = new ApiResponse<IEnumerable<ShipmentDto>>(shipmentsDto);

            return Ok(response);
        }

        [HttpGet("dapper/1")]
        public async Task<IActionResult> GetPostCommentUserAsync()
        {
            var shipments = await _shipmentService.GetShipmentCustomerRouteAsync();


            var response = new ApiResponse<IEnumerable<ShipmentCustomerRouteResponse>>(shipments);

            return Ok(response);
        }

        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetShipmentDtoMapperId(int id)
        {
            #region Validaciones
            var validationRequest = new GetByIdRequest { Id = id };
            var validationResult = await _validationService.ValidateAsync(validationRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Error de validacion del ID",
                    Errors = validationResult.Errors
                });
            }
            #endregion
            var shipment = await _shipmentService.GetByIdAsync(id);

            if (shipment == null)
            {
                return NotFound(new { Message = "No se encontró el envío con ese ID" });
            }
         
            var shipmentDto = _mapper.Map<ShipmentDto>(shipment);

            var response = new ApiResponse<ShipmentDto>(shipmentDto);

            return Ok(response);

        }

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertShipmentDtoMapper([FromBody] ShipmentDto shipmentDto)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var result = await _validationService.ValidateAsync(shipmentDto);
                if (!result.IsValid)
                {
                    return BadRequest(new { Errors = result.Errors });

                }
                #endregion

                var shipment = _mapper.Map<Shipment>(shipmentDto);
                shipment.Id = 0;
                await _shipmentService.InsertAsync(shipment);

                var response = new ApiResponse<Shipment>(shipment);

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Errors = new[] { ex.Message } });
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateShipmentDtoMapper(int id,
            [FromBody] ShipmentDto shipmentDto)
        {
            if (id != shipmentDto.Id)
            {
                return BadRequest("El ID del envio no coincide");
            }

            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound("Envio no encontrado");
            }

            _mapper.Map(shipmentDto, shipment);

            if (id != shipmentDto.Id)
                return BadRequest("El ID del envío no coincide con el de la URL.");


            await _shipmentService.UpdateAsync(shipment);

            var response = new ApiResponse<Shipment>(shipment);

            return Ok(response);
        }

        //No es necesario mapear para delete
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteshipmentDtoMapper(int id)
        {
            try
            {
                var shipment = await _shipmentService.GetByIdAsync(id);
                if (shipment == null)
                    return NotFound("Envio no encontrado");

                await _shipmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
    }
}