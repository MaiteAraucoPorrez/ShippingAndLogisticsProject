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
            CreateMap<Address, AddressDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    Enum.Parse<AddressType>(src.Type, true)));

            CreateMap<Warehouse, WarehouseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    Enum.Parse<WarehouseType>(src.Type, true)));

            CreateMap<ShipmentWarehouse, ShipmentWarehouseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    Enum.Parse<WarehouseShipmentStatus>(src.Status, true)));

            CreateMap<Driver, DriverDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    Enum.Parse<DriverStatus>(src.Status, true)));

            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                    src.FuelType.HasValue ? src.FuelType.Value.ToString() : null))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                    Enum.Parse<VehicleType>(src.Type, true)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    Enum.Parse<VehicleStatus>(src.Status, true)))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.FuelType) ? Enum.Parse<FuelType>(src.FuelType, true) : (FuelType?)null));

            CreateMap<Security, SecurityDto>().ReverseMap();
        }
    }
}
