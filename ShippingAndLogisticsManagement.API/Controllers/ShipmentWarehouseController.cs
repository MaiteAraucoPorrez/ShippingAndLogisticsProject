using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingAndLogisticsManagement.Api.Responses;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Net;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Controller for managing shipment movement through warehouses
    /// </summary>
    /// <remarks>
    /// Este controlador maneja el registro de entrada y salida de envíos
    /// en los almacenes del sistema logístico (check-in/dispatch).
    /// </remarks>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ShipmentWarehouseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShipmentWarehouseController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene el historial completo de almacenes por los que pasó un envío
        /// </summary>
        /// <remarks>
        /// Muestra la ruta completa del envío a través de la red de almacenes.
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/shipmentwarehouse/history/5
        /// </remarks>
        /// <param name="shipmentId">ID del envío</param>
        /// <returns>Historial de movimientos</returns>
        [HttpGet("history/{shipmentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentWarehouseHistoryResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        public async Task<IActionResult> GetShipmentHistory(int shipmentId)
        {
            try
            {
                if (shipmentId <= 0)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "ID de envío inválido" } }
                    });

                var history = await _unitOfWork.ShipmentWarehouseRepository
                    .GetShipmentHistoryAsync(shipmentId);

                if (!history.Any())
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Warning", Description = "El envío no tiene historial de almacenes" } }
                    });

                return Ok(new ApiResponse<IEnumerable<ShipmentWarehouseHistoryResponse>>(history)
                {
                    Messages = new[] { new Message { Type = "Information", Description = $"Se encontraron {history.Count()} registros en el historial" } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Obtiene todos los envíos actualmente en un almacén
        /// </summary>
        /// <remarks>
        /// Lista los envíos que están actualmente almacenados (sin fecha de salida).
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/shipmentwarehouse/current/2
        /// </remarks>
        /// <param name="warehouseId">ID del almacén</param>
        /// <returns>Lista de envíos en el almacén</returns>
        [HttpGet("current/{warehouseId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ShipmentWarehouseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        public async Task<IActionResult> GetCurrentShipmentsInWarehouse(int warehouseId)
        {
            try
            {
                if (warehouseId <= 0)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "ID de almacén inválido" } }
                    });

                var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
                if (warehouse == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El almacén no existe" } }
                    });

                var currentShipments = await _unitOfWork.ShipmentWarehouseRepository
                    .GetCurrentShipmentsInWarehouseAsync(warehouseId);

                var shipmentsDto = _mapper.Map<IEnumerable<ShipmentWarehouseDto>>(currentShipments);

                return Ok(new ApiResponse<IEnumerable<ShipmentWarehouseDto>>(shipmentsDto)
                {
                    Messages = new[] { new Message { Type = "Information", Description = $"Hay {shipmentsDto.Count()} envíos en este almacén" } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Obtiene la ubicación actual de un envío
        /// </summary>
        /// <remarks>
        /// Indica en qué almacén está actualmente el envío (si está en alguno).
        /// 
        /// Ejemplo de uso:
        /// GET /api/v1/shipmentwarehouse/location/5
        /// </remarks>
        /// <param name="shipmentId">ID del envío</param>
        /// <returns>Ubicación actual o null si no está en almacén</returns>
        [HttpGet("location/{shipmentId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ShipmentWarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        public async Task<IActionResult> GetCurrentLocation(int shipmentId)
        {
            try
            {
                if (shipmentId <= 0)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "ID de envío inválido" } }
                    });

                var currentLocation = await _unitOfWork.ShipmentWarehouseRepository
                    .GetCurrentWarehouseForShipmentAsync(shipmentId);

                if (currentLocation == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Information", Description = "El envío no está actualmente en ningún almacén" } }
                    });

                var locationDto = _mapper.Map<ShipmentWarehouseDto>(currentLocation);

                return Ok(new ApiResponse<ShipmentWarehouseDto>(locationDto)
                {
                    Messages = new[] { new Message { Type = "Information", Description = "Ubicación actual del envío" } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new[] { new Message { Type = "Error", Description = ex.Message } }
                });
            }
        }

        /// <summary>
        /// Registra la ENTRADA de un envío a un almacén (Check-in)
        /// </summary>
        /// <remarks>
        /// Este endpoint registra cuando un envío llega a un almacén.
        /// Valida que:
        /// - El envío exista y NO esté ya en otro almacén
        /// - El almacén exista y esté activo
        /// - El almacén tenga capacidad disponible
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/shipmentwarehouse/checkin
        /// {
        ///   "shipmentId": 5,
        ///   "warehouseId": 2,
        ///   "entryDate": "2025-01-15T10:30:00",
        ///   "status": "Received",
        ///   "receivedBy": "Juan Pérez",
        ///   "storageLocation": "Estante A-12"
        /// }
        /// </remarks>
        /// <param name="dto">Datos del registro de entrada</param>
        /// <returns>Confirmación del registro</returns>
        /// <response code="201">Entrada registrada exitosamente</response>
        /// <response code="400">Si hay errores de validación</response>
        /// <response code="404">Si el envío o almacén no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("checkin")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<ShipmentWarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        public async Task<IActionResult> RegisterEntry([FromBody] ShipmentWarehouseDto dto)
        {
            try
            {
                // 1. Validar que el envío existe
                var shipment = await _unitOfWork.ShipmentRepository.GetById(dto.ShipmentId);
                if (shipment == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El envío no existe" } }
                    });

                // 2. Validar que el almacén existe y está activo
                var warehouse = await _unitOfWork.WarehouseRepository.GetById(dto.WarehouseId);
                if (warehouse == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El almacén no existe" } }
                    });

                if (!warehouse.IsActive)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El almacén está inactivo" } }
                    });

                // 3. Validar que el envío NO esté ya en otro almacén
                var isInWarehouse = await _unitOfWork.ShipmentWarehouseRepository
                    .IsShipmentInWarehouseAsync(dto.ShipmentId);

                if (isInWarehouse)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El envío ya está en un almacén. Primero debe registrar su salida." } }
                    });

                // 4. Validar capacidad disponible (aproximado por cantidad de envíos)
                var currentShipments = await _unitOfWork.ShipmentWarehouseRepository
                    .GetCurrentShipmentsInWarehouseAsync(dto.WarehouseId);

                var availableCapacity = warehouse.MaxCapacityM3 - warehouse.CurrentCapacityM3;
                if (availableCapacity < 1) // Simplificado: 1m³ por envío
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El almacén no tiene capacidad disponible" } }
                    });

                // 5. Crear el registro de entrada
                var shipmentWarehouse = _mapper.Map<ShipmentWarehouse>(dto);
                shipmentWarehouse.EntryDate = dto.EntryDate == default ? DateTime.Now : dto.EntryDate;
                shipmentWarehouse.Status = WarehouseShipmentStatus.Received;
                shipmentWarehouse.ExitDate = null; // Importante: sin fecha de salida

                await _unitOfWork.ShipmentWarehouseRepository.RegisterEntryAsync(shipmentWarehouse);
                await _unitOfWork.SaveChangesAsync();

                // 6. Actualizar capacidad del almacén (simplificado)
                warehouse.CurrentCapacityM3 += 1; // Simplificado
                await _unitOfWork.WarehouseRepository.Update(warehouse);
                await _unitOfWork.SaveChangesAsync();

                var createdDto = _mapper.Map<ShipmentWarehouseDto>(shipmentWarehouse);

                return StatusCode(201, new ApiResponse<ShipmentWarehouseDto>(createdDto)
                {
                    Messages = new[] { new Message { Type = "Success", Description = "Entrada al almacén registrada exitosamente" } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new[] { new Message { Type = "Error", Description = $"Error: {ex.Message}" } }
                });
            }
        }

        /// <summary>
        /// Registra la SALIDA de un envío de un almacén (Dispatch)
        /// </summary>
        /// <remarks>
        /// Este endpoint registra cuando un envío sale de un almacén.
        /// Valida que:
        /// - El envío esté actualmente en el almacén
        /// - La fecha de salida sea posterior a la entrada
        /// 
        /// Ejemplo de solicitud:
        /// POST /api/v1/shipmentwarehouse/dispatch
        /// {
        ///   "shipmentWarehouseId": 15,
        ///   "exitDate": "2025-01-16T14:00:00",
        ///   "dispatchedBy": "María Rodríguez"
        /// }
        /// </remarks>
        /// <param name="request">Datos del despacho</param>
        /// <returns>Confirmación del despacho</returns>
        /// <response code="200">Salida registrada exitosamente</response>
        /// <response code="400">Si hay errores de validación</response>
        /// <response code="404">Si el registro no existe</response>
        [HttpPost("dispatch")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ShipmentWarehouseDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ResponseData))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ResponseData))]
        public async Task<IActionResult> RegisterExit([FromBody] DispatchRequest request)
        {
            try
            {
                // 1. Validar que el registro existe
                var shipmentWarehouse = await _unitOfWork.ShipmentWarehouseRepository
                    .GetById(request.ShipmentWarehouseId);

                if (shipmentWarehouse == null)
                    return NotFound(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "El registro de envío-almacén no existe" } }
                    });

                // 2. Validar que NO tenga ya fecha de salida
                if (shipmentWarehouse.ExitDate.HasValue)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "Este envío ya fue despachado anteriormente" } }
                    });

                // 3. Validar fecha de salida
                var exitDate = request.ExitDate == default ? DateTime.Now : request.ExitDate;

                if (exitDate < shipmentWarehouse.EntryDate)
                    return BadRequest(new ResponseData
                    {
                        Messages = new[] { new Message { Type = "Error", Description = "La fecha de salida no puede ser anterior a la entrada" } }
                    });

                // 4. Registrar salida
                await _unitOfWork.ShipmentWarehouseRepository.RegisterExitAsync(
                    request.ShipmentWarehouseId,
                    exitDate,
                    request.DispatchedBy ?? "Sistema"
                );
                await _unitOfWork.SaveChangesAsync();

                // 5. Actualizar capacidad del almacén
                var warehouse = await _unitOfWork.WarehouseRepository
                    .GetById(shipmentWarehouse.WarehouseId);

                if (warehouse != null)
                {
                    warehouse.CurrentCapacityM3 = Math.Max(0, warehouse.CurrentCapacityM3 - 1);
                    await _unitOfWork.WarehouseRepository.Update(warehouse);
                    await _unitOfWork.SaveChangesAsync();
                }

                // 6. Obtener registro actualizado
                var updated = await _unitOfWork.ShipmentWarehouseRepository
                    .GetById(request.ShipmentWarehouseId);

                var updatedDto = _mapper.Map<ShipmentWarehouseDto>(updated);

                return Ok(new ApiResponse<ShipmentWarehouseDto>(updatedDto)
                {
                    Messages = new[] { new Message { Type = "Success", Description = "Salida del almacén registrada exitosamente" } }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseData
                {
                    Messages = new[] { new Message { Type = "Error", Description = $"Error: {ex.Message}" } }
                });
            }
        }
    }

    /// <summary>
    /// Request model for dispatch operation
    /// </summary>
    public class DispatchRequest
    {
        public int ShipmentWarehouseId { get; set; }
        public DateTime ExitDate { get; set; }
        public string? DispatchedBy { get; set; }
    }
}