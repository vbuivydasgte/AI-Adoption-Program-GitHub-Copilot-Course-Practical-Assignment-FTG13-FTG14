using AutoMapper;
using WarehouseManagement.Application.Common;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public class ProductService : CrudServiceBase<Product, ProductDto>, IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository, IMapper mapper)
        : base(productRepository, mapper)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        return await GetAllAsync();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        return await CreateAsync(createProductDto, ValidateBeforeCreateAsync);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        return await UpdateAsync(id, updateProductDto, ApplyUpdates, ValidateBeforeUpdateAsync);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await DeleteAsync(id);
    }

    private async Task ValidateBeforeCreateAsync(CreateProductDto createDto)
    {
        var existingProductWithSku = await _productRepository.GetBySKUAsync(createDto.SKU);
        if (existingProductWithSku != null)
        {
            throw new InvalidOperationException(ValidationMessages.ProductSkuAlreadyExists);
        }
    }

    private async Task ValidateBeforeUpdateAsync(int id, Product existingEntity, UpdateProductDto updateDto)
    {
        if (!string.Equals(existingEntity.SKU, updateDto.SKU, StringComparison.OrdinalIgnoreCase))
        {
            var existingProductWithSku = await _productRepository.GetBySKUAsync(updateDto.SKU);
            if (existingProductWithSku != null && existingProductWithSku.Id != id)
            {
                throw new InvalidOperationException(ValidationMessages.ProductSkuAlreadyExists);
            }
        }
    }

    private static void ApplyUpdates(Product existingEntity, UpdateProductDto updateDto)
    {
        existingEntity.Name = updateDto.Name;
        existingEntity.SKU = updateDto.SKU;
        existingEntity.Description = updateDto.Description;
        existingEntity.ModifiedAt = DateTime.UtcNow;
    }
}
