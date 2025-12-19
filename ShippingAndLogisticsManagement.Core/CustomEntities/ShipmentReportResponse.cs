using System.Globalization;

namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    public class ShipmentReportResponse
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public int Quantity { get; set; }
        public string State { get; set; }

        public string FormattedReport => $"{Date:d-MMM}/{DayName}/{Quantity}/{State}";
    }
}