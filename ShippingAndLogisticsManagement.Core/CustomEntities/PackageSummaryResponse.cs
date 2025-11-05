namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Represents a statistical summary of packages within a specific shipment.
    /// Includes totals and averages of weight and value.
    /// </summary>
    public class PackageSummaryResponse
    {
        /// <summary>
        /// ID of the shipment.
        /// </summary>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Total number of packages within the shipment.
        /// </summary>
        public int TotalPackages { get; set; }

        /// <summary>
        /// Sum of all package weights.
        /// </summary>
        public double TotalWeight { get; set; }

        /// <summary>
        /// Sum of all package prices (total value).
        /// </summary>
        public double TotalValue { get; set; }

        /// <summary>
        /// Average package weight.
        /// </summary>
        public double AvgWeight { get; set; }

        /// <summary>
        /// Average package price.
        /// </summary>
        public double AvgValue { get; set; }
    }
}
