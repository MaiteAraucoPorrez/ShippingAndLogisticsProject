using AutoMapper;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Shipment, ShipmentDto>().ReverseMap();
            CreateMap<Package, PackageDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<Warehouse, WarehouseDto>().ReverseMap();
            CreateMap<ShipmentWarehouse, ShipmentWarehouseDto>().ReverseMap();
            CreateMap<Driver, DriverDto>().ReverseMap();
            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<Security, SecurityDto>().ReverseMap();
        }
    }
}
