using AutoMapper;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Shipment, ShipmentDto>();
            CreateMap<ShipmentDto, Shipment>();
        }
    }
}
