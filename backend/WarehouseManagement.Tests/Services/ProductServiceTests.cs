using AutoMapper;
using NSubstitute;
using NUnit.Framework;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Tests.Services;

[TestFixture]
public class ProductServiceTests
{
    private IProductRepository _productRepository = null!;
    private IMapper _mapper = null!;
    private ProductService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _service = new ProductService(_productRepository, _mapper);
    }

    [Test]
    public void CreateProductAsync_WhenSkuAlreadyExists_ThrowsInvalidOperationException()
    {
        var createDto = new CreateProductDto
        {
            Name = "Product",
            SKU = "SKU-1",
            Description = "Desc"
        };

        _productRepository.GetBySKUAsync(createDto.SKU).Returns(new Product { Id = 50, SKU = createDto.SKU });

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateProductAsync(createDto));

        Assert.That(ex!.Message, Is.EqualTo("Product SKU already exists."));
        _productRepository.DidNotReceive().AddAsync(Arg.Any<Product>());
    }

    [Test]
    public async Task CreateProductAsync_WhenSkuIsUnique_AddsProductAndReturnsDto()
    {
        var createDto = new CreateProductDto
        {
            Name = "Product",
            SKU = "SKU-2",
            Description = "Desc"
        };

        var mappedProduct = new Product { Name = createDto.Name, SKU = createDto.SKU, Description = createDto.Description };
        var createdProduct = new Product { Id = 2, Name = createDto.Name, SKU = createDto.SKU, Description = createDto.Description };
        var expectedDto = new ProductDto { Id = createdProduct.Id, Name = createdProduct.Name, SKU = createdProduct.SKU, Description = createdProduct.Description };

        _productRepository.GetBySKUAsync(createDto.SKU).Returns((Product?)null);
        _mapper.Map<Product>(createDto).Returns(mappedProduct);
        _productRepository.AddAsync(mappedProduct).Returns(createdProduct);
        _mapper.Map<ProductDto>(createdProduct).Returns(expectedDto);

        var result = await _service.CreateProductAsync(createDto);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.SKU, Is.EqualTo("SKU-2"));
        await _productRepository.Received(1).AddAsync(mappedProduct);
    }

    [Test]
    public void UpdateProductAsync_WhenSkuBelongsToAnotherProduct_ThrowsInvalidOperationException()
    {
        var updateDto = new UpdateProductDto
        {
            Name = "Updated",
            SKU = "SKU-NEW",
            Description = "Updated Desc"
        };

        _productRepository.GetByIdAsync(1).Returns(new Product { Id = 1, Name = "Current", SKU = "SKU-OLD", Description = "Desc" });
        _productRepository.GetBySKUAsync(updateDto.SKU).Returns(new Product { Id = 2, SKU = updateDto.SKU });

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateProductAsync(1, updateDto));

        Assert.That(ex!.Message, Is.EqualTo("Product SKU already exists."));
    }

    [Test]
    public async Task GetAllProductsAsync_ReturnsMappedProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "A", SKU = "SKU-A" },
            new() { Id = 2, Name = "B", SKU = "SKU-B" }
        };

        var mapped = new List<ProductDto>
        {
            new() { Id = 1, Name = "A", SKU = "SKU-A" },
            new() { Id = 2, Name = "B", SKU = "SKU-B" }
        };

        _productRepository.GetAllAsync().Returns(products);
        _mapper.Map<IEnumerable<ProductDto>>(products).Returns(mapped);

        var result = (await _service.GetAllProductsAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[1].SKU, Is.EqualTo("SKU-B"));
    }

    [Test]
    public async Task GetProductByIdAsync_WhenFound_ReturnsMappedProduct()
    {
        var product = new Product { Id = 7, Name = "Desk", SKU = "SKU-7" };
        var mapped = new ProductDto { Id = 7, Name = "Desk", SKU = "SKU-7" };

        _productRepository.GetByIdAsync(7).Returns(product);
        _mapper.Map<ProductDto>(product).Returns(mapped);

        var result = await _service.GetProductByIdAsync(7);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(7));
    }

    [Test]
    public async Task GetProductByIdAsync_WhenMissing_ReturnsNull()
    {
        _productRepository.GetByIdAsync(70).Returns((Product?)null);

        var result = await _service.GetProductByIdAsync(70);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateProductAsync_WhenProductMissing_ReturnsNull()
    {
        var updateDto = new UpdateProductDto { Name = "X", SKU = "X", Description = "X" };
        _productRepository.GetByIdAsync(22).Returns((Product?)null);

        var result = await _service.UpdateProductAsync(22, updateDto);

        Assert.That(result, Is.Null);
        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>());
    }

    [Test]
    public async Task UpdateProductAsync_WhenValid_UpdatesAndReturnsMappedDto()
    {
        var existing = new Product
        {
            Id = 1,
            Name = "Old",
            SKU = "SKU-OLD",
            Description = "Old Desc",
            ModifiedAt = DateTime.UtcNow.AddDays(-2)
        };

        var updateDto = new UpdateProductDto
        {
            Name = "New",
            SKU = "SKU-OLD",
            Description = "New Desc"
        };

        var mapped = new ProductDto { Id = 1, Name = "New", SKU = "SKU-OLD", Description = "New Desc" };

        _productRepository.GetByIdAsync(1).Returns(existing);
        _mapper.Map<ProductDto>(existing).Returns(mapped);

        var result = await _service.UpdateProductAsync(1, updateDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("New"));
        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p =>
            p.Id == 1 && p.Name == "New" && p.Description == "New Desc"));
    }

    [Test]
    public async Task UpdateProductAsync_WhenSkuChangesButBelongsToSameProduct_UpdatesSuccessfully()
    {
        var existing = new Product
        {
            Id = 9,
            Name = "Old",
            SKU = "SKU-OLD",
            Description = "Old Desc"
        };

        var updateDto = new UpdateProductDto
        {
            Name = "Renamed",
            SKU = "SKU-NEW",
            Description = "New Desc"
        };

        var mapped = new ProductDto { Id = 9, Name = "Renamed", SKU = "SKU-NEW", Description = "New Desc" };

        _productRepository.GetByIdAsync(9).Returns(existing);
        _productRepository.GetBySKUAsync("SKU-NEW").Returns(new Product { Id = 9, SKU = "SKU-NEW" });
        _mapper.Map<ProductDto>(existing).Returns(mapped);

        var result = await _service.UpdateProductAsync(9, updateDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.SKU, Is.EqualTo("SKU-NEW"));
        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p => p.Id == 9 && p.SKU == "SKU-NEW"));
    }

    [Test]
    public async Task DeleteProductAsync_WhenProductExists_DeletesAndReturnsTrue()
    {
        var product = new Product { Id = 12, Name = "Mouse", SKU = "SKU-M" };
        _productRepository.GetByIdAsync(12).Returns(product);

        var result = await _service.DeleteProductAsync(12);

        Assert.That(result, Is.True);
        await _productRepository.Received(1).DeleteAsync(product);
    }

    [Test]
    public async Task DeleteProductAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        _productRepository.GetByIdAsync(99).Returns((Product?)null);

        var result = await _service.DeleteProductAsync(99);

        Assert.That(result, Is.False);
        await _productRepository.DidNotReceive().DeleteAsync(Arg.Any<Product>());
    }
}
