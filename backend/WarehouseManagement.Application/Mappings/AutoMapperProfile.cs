using AutoMapper;
using WarehouseManagement.Application.DTOs.History;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Stock;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Warehouse mappings
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseDto, Warehouse>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<UpdateWarehouseDto, Warehouse>()
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Stock mappings
        CreateMap<Stock, StockDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.Product.SKU))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name));

        // History mappings
        CreateMap<StockHistory, StockHistoryDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Stock != null && src.Stock.Product != null ? src.Stock.Product.Name : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Stock != null && src.Stock.Warehouse != null ? src.Stock.Warehouse.Name : string.Empty));
    }
}
