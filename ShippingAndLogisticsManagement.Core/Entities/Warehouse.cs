namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un almacén o centro de distribución en el sistema logístico
    /// </summary>
    /// <remarks>
    /// Los almacenes son puntos estratégicos donde se reciben, almacenan y despachan
    /// los envíos. Funcionan como nodos en la red de distribución.
    /// 
    /// Tipos de almacenes:
    /// - Central: Almacén principal que recibe y distribuye a nivel nacional
    /// - Regional: Almacenes intermedios por departamento
    /// - Local: Puntos de distribución final en ciudades
    /// 
    /// Reglas de negocio:
    /// - Código único por almacén
    /// - Capacidad actual no puede exceder capacidad máxima
    /// - Ciudad y departamento obligatorios
    /// - Solo almacenes activos pueden recibir nuevos envíos
    /// </remarks>
    public class Warehouse : BaseEntity
    {
        /// <summary>
        /// Identificador único del almacén
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Nombre descriptivo del almacén
        /// </summary>
        /// <remarks>
        /// Debe ser único y descriptivo de la ubicación.
        /// </remarks>
        /// <example>Almacén Central La Paz</example>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Código único del almacén
        /// </summary>
        /// <remarks>
        /// Código alfanumérico para identificación rápida.
        /// Formato sugerido: WH-[CIUDAD]-[NÚMERO]
        /// </remarks>
        /// <example>WH-LP-001</example>
        public string Code { get; set; } = null!;

        /// <summary>
        /// Dirección completa del almacén
        /// </summary>
        /// <remarks>
        /// Incluir calle, número, zona y referencias.
        /// </remarks>
        /// <example>Av. Buenos Aires Km 7.5, Zona Industrial</example>
        public string Address { get; set; } = null!;

        /// <summary>
        /// Ciudad donde se ubica el almacén
        /// </summary>
        /// <example>La Paz</example>
        public string City { get; set; } = null!;

        /// <summary>
        /// Departamento de Bolivia
        /// </summary>
        /// <example>La Paz</example>
        public string Department { get; set; } = null!;

        /// <summary>
        /// Teléfono de contacto del almacén
        /// </summary>
        /// <example>2-2234567</example>
        public string Phone { get; set; } = null!;

        /// <summary>
        /// Email del almacén (opcional)
        /// </summary>
        /// <example>almacen.lapaz@empresa.com</example>
        public string? Email { get; set; }

        /// <summary>
        /// Capacidad máxima en metros cúbicos
        /// </summary>
        /// <remarks>
        /// Representa el volumen total que puede almacenar.
        /// </remarks>
        /// <example>1000.0</example>
        public double MaxCapacityM3 { get; set; }

        /// <summary>
        /// Capacidad actual utilizada en metros cúbicos
        /// </summary>
        /// <remarks>
        /// Se actualiza automáticamente al recibir/despachar envíos.
        /// No puede exceder MaxCapacityM3.
        /// </remarks>
        /// <example>650.5</example>
        public double CurrentCapacityM3 { get; set; }

        /// <summary>
        /// Indica si el almacén está activo y operativo
        /// </summary>
        /// <remarks>
        /// Solo almacenes activos pueden recibir nuevos envíos.
        /// </remarks>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Tipo de almacén según su función en la red
        /// </summary>
        /// <example>Regional</example>
        public WarehouseType Type { get; set; }

        /// <summary>
        /// Horario de operación del almacén
        /// </summary>
        /// <example>Lunes a Viernes 8:00-18:00, Sábados 8:00-13:00</example>
        public string? OperatingHours { get; set; }

        /// <summary>
        /// Nombre del encargado del almacén
        /// </summary>
        /// <example>Juan Pérez</example>
        public string? ManagerName { get; set; }

        /// <summary>
        /// Latitud GPS del almacén
        /// </summary>
        /// <example>-16.5000</example>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitud GPS del almacén
        /// </summary>
        /// <example>-68.1500</example>
        public double? Longitude { get; set; }

        /// <summary>
        /// Fecha de apertura del almacén
        /// </summary>
        public DateTime OpeningDate { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Notas adicionales sobre el almacén
        /// </summary>
        /// <example>Cuenta con rampa para carga pesada</example>
        public string? Notes { get; set; }

        /// <summary>
        /// Relación con envíos que han pasado por este almacén
        /// </summary>
        public ICollection<ShipmentWarehouse> ShipmentWarehouses { get; set; }
            = new List<ShipmentWarehouse>();
    }

    /// <summary>
    /// Tipo de almacén según su nivel en la red de distribución
    /// </summary>
    public enum WarehouseType
    {
        /// <summary>
        /// Almacén principal - Hub nacional
        /// </summary>
        Central = 1,

        /// <summary>
        /// Almacén regional - Hub departamental
        /// </summary>
        Regional = 2,

        /// <summary>
        /// Almacén local - Distribución final
        /// </summary>
        Local = 3
    }
}