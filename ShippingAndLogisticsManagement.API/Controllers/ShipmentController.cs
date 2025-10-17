using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using ShippingAndLogisticsManagement.Infrastructure.Validator;
using System.Net;


namespace ShippingAndLogisticsManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shipments = await _shipmentService.GetAllAsync();
            return Ok(shipments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Shipment shipment)
        {
            await _shipmentService.InsertAsync(shipment);
            return Ok(shipment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null) return NotFound();

            await _shipmentService.DeleteAsync(shipment);
            return Ok();
        }
        #endregion

        #region CRUD Operations Con DTOs
         [HttpGet("dto")]
         public async Task<IActionResult> GetShipmentsDto()
         {
             var shipments = await _shipmentService.GetAllAsync();
            var shipmentsDto = shipments.Select(s => new ShipmentDto
            {
                Id = s.Id,
                ShippingDate = s.ShippingDate.ToString("dd-MM-yyyy"),
                State = s.State,
                CustomerId = s.CustomerId,
                RouteId = s.RouteId,
                TotalCost = s.TotalCost,
                TrackingNumber = s.TrackingNumber
            });
             return Ok(shipmentsDto);
         }


         [HttpGet("dto/{id}")]
         public async Task<IActionResult> GetShipmentIdDto(int id)
         {
             var shipment = await _shipmentService.GetByIdAsync(id);
             var shipmentDto = new ShipmentDto
             {
                 Id = shipment.Id,
                 ShippingDate = shipment.ShippingDate.ToString("dd-MM-yyyy"),
                 State = shipment.State,
                 CustomerId = shipment.CustomerId,
                 RouteId = shipment.RouteId,
                 TotalCost = shipment.TotalCost,
                 TrackingNumber = shipment.TrackingNumber
             };

             return Ok(shipmentDto);
         }


         [HttpPost("dto")]
         public async Task<IActionResult> InsertPostDto(ShipmentDto shipmentDto)
         {
             var shipment = new Shipment
             {
                 Id = shipmentDto.Id,
                 ShippingDate = Convert.ToDateTime(shipmentDto.ShippingDate),
                 State = shipmentDto.State,
                 CustomerId = shipmentDto.CustomerId,
                 RouteId = shipmentDto.RouteId,
                 TotalCost = shipmentDto.TotalCost,
                 TrackingNumber = shipmentDto.TrackingNumber
             };
             await _shipmentService.InsertAsync(shipment);
             return Ok(shipment);
         }


         [HttpPut("dto/{id}")]
         public async Task<IActionResult> UpdateShipmentDto(int id,
             [FromBody] ShipmentDto shipmentDto)
         {
              if (id != shipmentDto.Id)
                 return BadRequest("El Id del envio no coincide");

               var shipment = await _shipmentService.GetByIdAsync(id);
               if (shipment == null)
                   return NotFound("Envio no encontrado");

               shipment.Id = shipmentDto.Id;
               shipment.ShippingDate = Convert.ToDateTime(shipmentDto.ShippingDate);
               shipment.State = shipmentDto.State;
               shipment.CustomerId = shipmentDto.CustomerId;
               shipment.RouteId = shipmentDto.RouteId;
               shipment.TotalCost = shipmentDto.TotalCost;
               shipment.TrackingNumber = shipmentDto.TrackingNumber;

            await _shipmentService.UpdateAsync(shipment);
               return Ok(shipment);
         }


         [HttpDelete("dto/{id}")]
         public async Task<IActionResult> DeleteShipmentDto(int id)
         {
             var shipment = await _shipmentService.GetByIdAsync(id);
             if (shipment == null)
                 return NotFound("Envio no encontrado");

             await _shipmentService.DeleteAsync(shipment);
             return NoContent();
         }
        #endregion

        #region ConMapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetShipmentsDtoMapper()
        {
            var shipments = await _shipmentService.GetAllAsync();
            var shipmentsDto = _mapper.Map<IEnumerable<ShipmentDto>>(shipments);

            var response = new ApiResponse<IEnumerable<ShipmentDto>>(shipmentsDto);

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
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound("Envio no encontrado");
            }

            try
            {
                await _shipmentService.DeleteAsync(shipment);
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