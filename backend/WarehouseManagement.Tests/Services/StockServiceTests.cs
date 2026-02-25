using AutoMapper;
using NSubstitute;
using NUnit.Framework;
using WarehouseManagement.Application.DTOs.Stock;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;
using WarehouseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace WarehouseManagement.Tests.Services;

[TestFixture]
public class StockServiceTests
{
    private IStockRepository _stockRepository = null!;
    private IHistoryRepository _historyRepository = null!;
    private ApplicationDbContext _dbContext = null!;
    private IMapper _mapper = null!;
    private StockService _service = null!;
    private IDbContextTransaction _mockTransaction = null!;

    [SetUp]
    public void SetUp()
    {
        _stockRepository = Substitute.For<IStockRepository>();
        _historyRepository = Substitute.For<IHistoryRepository>();
        _dbContext = Substitute.For<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
        _mapper = Substitute.For<IMapper>();
        
        // Mock transaction
        _mockTransaction = Substitute.For<IDbContextTransaction>();
        var mockDatabase = Substitute.For<DatabaseFacade>(_dbContext);
        mockDatabase.BeginTransactionAsync(default).ReturnsForAnyArgs(_mockTransaction);
        _dbContext.Database.Returns(mockDatabase);
        
        _service = new StockService(_stockRepository, _historyRepository, _dbContext, _mapper);
    }

    [Test]
    public void AdjustStockAsync_WhenNewStockWouldBeNegative_ThrowsInvalidOperationException()
    {
        var adjustment = new StockAdjustmentDto
        {
            ProductId = 1,
            WarehouseId = 1,
            QuantityChange = -1,
            ChangedBy = "worker",
            Reason = "Adjustment"
        };

        _stockRepository.GetByProductAndWarehouseAsync(1, 1).Returns((Stock?)null);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdjustStockAsync(adjustment));

        Assert.That(ex!.Message, Is.EqualTo("Stock quantity cannot be negative."));
        _stockRepository.DidNotReceive().AddAsync(Arg.Any<Stock>());
    }

    [Test]
    public void AdjustStockAsync_WhenExistingStockWouldBeNegative_ThrowsInvalidOperationException()
    {
        var adjustment = new StockAdjustmentDto
        {
            ProductId = 1,
            WarehouseId = 1,
            QuantityChange = -10,
            ChangedBy = "worker",
            Reason = "Adjustment"
        };

        _stockRepository.GetByProductAndWarehouseAsync(1, 1).Returns(new Stock { Id = 7, ProductId = 1, WarehouseId = 1, Quantity = 2 });

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdjustStockAsync(adjustment));

        Assert.That(ex!.Message, Is.EqualTo("Stock quantity cannot be negative."));
        _stockRepository.DidNotReceive().UpdateAsync(Arg.Any<Stock>());
    }

    [Test]
    public async Task AdjustStockAsync_WhenExistingStockValid_UpdatesStockAndReturnsDto()
    {
        var adjustment = new StockAdjustmentDto
        {
            ProductId = 5,
            WarehouseId = 3,
            QuantityChange = 4,
            ChangedBy = "worker",
            Reason = "Receive"
        };

        var stock = new Stock
        {
            Id = 11,
            ProductId = 5,
            WarehouseId = 3,
            Quantity = 6,
            LastUpdated = DateTime.UtcNow.AddDays(-1)
        };

        var expectedDto = new StockDto
        {
            Id = 11,
            ProductId = 5,
            WarehouseId = 3,
            Quantity = 10
        };

        _stockRepository.GetByProductAndWarehouseAsync(5, 3).Returns(stock);
        _stockRepository.GetByIdWithDetailsAsync(11).Returns(stock);
        _mapper.Map<StockDto>(stock).Returns(expectedDto);

        var result = await _service.AdjustStockAsync(adjustment);

        Assert.That(result.Quantity, Is.EqualTo(10));
        await _stockRepository.Received(1).UpdateAsync(Arg.Is<Stock>(s => s.Id == 11 && s.Quantity == 10));
        await _historyRepository.Received(1).AddStockHistoryAsync(
            Arg.Is<StockHistory>(h => h.StockId == 11 && h.PreviousQuantity == 6 && h.NewQuantity == 10));
    }

    [Test]
    public async Task AdjustStockAsync_WhenStockDoesNotExistAndChangePositive_CreatesStockAndReturnsDto()
    {
        var adjustment = new StockAdjustmentDto
        {
            ProductId = 2,
            WarehouseId = 8,
            QuantityChange = 5,
            ChangedBy = "worker",
            Reason = "Initial load"
        };

        var createdStock = new Stock
        {
            Id = 33,
            ProductId = 2,
            WarehouseId = 8,
            Quantity = 5,
            LastUpdated = DateTime.UtcNow
        };

        var expectedDto = new StockDto { Id = 33, ProductId = 2, WarehouseId = 8, Quantity = 5 };

        _stockRepository.GetByProductAndWarehouseAsync(2, 8).Returns((Stock?)null);
        _stockRepository.AddAsync(Arg.Any<Stock>()).Returns(createdStock);
        _stockRepository.GetByIdWithDetailsAsync(33).Returns(createdStock);
        _mapper.Map<StockDto>(createdStock).Returns(expectedDto);

        var result = await _service.AdjustStockAsync(adjustment);

        Assert.That(result.Id, Is.EqualTo(33));
        Assert.That(result.Quantity, Is.EqualTo(5));
        await _stockRepository.Received(1).AddAsync(Arg.Is<Stock>(s =>
            s.ProductId == 2 && s.WarehouseId == 8 && s.Quantity == 5));
        await _historyRepository.Received(1).AddStockHistoryAsync(Arg.Is<StockHistory>(h =>
            h.StockId == 33 && h.PreviousQuantity == 0 && h.NewQuantity == 5));
    }

    [Test]
    public async Task GetAllStockAsync_UsesOnlyStocksWithDetailsAndMapsResult()
    {
        var stocks = new List<Stock>
        {
            new() { Id = 1, ProductId = 1, WarehouseId = 1, Quantity = 3 },
            new() { Id = 2, ProductId = 2, WarehouseId = 2, Quantity = 4 }
        };

        var mapped = new List<StockDto> 
        { 
            new() { Id = 1, ProductId = 1, WarehouseId = 1, Quantity = 3 },
            new() { Id = 2, ProductId = 2, WarehouseId = 2, Quantity = 4 }
        };

        _stockRepository.GetAllWithDetailsAsync().Returns(stocks);
        _mapper.Map<IEnumerable<StockDto>>(stocks).Returns(mapped);

        var result = (await _service.GetAllStockAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[1].Id, Is.EqualTo(2));
    }

    [Test]
    public async Task GetStockByWarehouseAsync_ReturnsMappedStocks()
    {
        var stocks = new List<Stock> { new() { Id = 5, WarehouseId = 9, Quantity = 10 } };
        var mapped = new List<StockDto> { new() { Id = 5, WarehouseId = 9, Quantity = 10 } };

        _stockRepository.GetByWarehouseIdAsync(9).Returns(stocks);
        _mapper.Map<IEnumerable<StockDto>>(stocks).Returns(mapped);

        var result = (await _service.GetStockByWarehouseAsync(9)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].WarehouseId, Is.EqualTo(9));
    }

    [Test]
    public async Task GetStockByProductAsync_ReturnsMappedStocks()
    {
        var stocks = new List<Stock> { new() { Id = 8, ProductId = 15, Quantity = 20 } };
        var mapped = new List<StockDto> { new() { Id = 8, ProductId = 15, Quantity = 20 } };

        _stockRepository.GetByProductIdAsync(15).Returns(stocks);
        _mapper.Map<IEnumerable<StockDto>>(stocks).Returns(mapped);

        var result = (await _service.GetStockByProductAsync(15)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ProductId, Is.EqualTo(15));
    }

    [Test]
    public async Task GetStockByIdAsync_WhenFound_ReturnsMappedStock()
    {
        var stock = new Stock { Id = 77, ProductId = 1, WarehouseId = 1, Quantity = 5 };
        var mapped = new StockDto { Id = 77, ProductId = 1, WarehouseId = 1, Quantity = 5 };

        _stockRepository.GetByIdWithDetailsAsync(77).Returns(stock);
        _mapper.Map<StockDto>(stock).Returns(mapped);

        var result = await _service.GetStockByIdAsync(77);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(77));
    }

    [Test]
    public async Task GetStockByIdAsync_WhenMissing_ReturnsNull()
    {
        _stockRepository.GetByIdWithDetailsAsync(770).Returns((Stock?)null);

        var result = await _service.GetStockByIdAsync(770);

        Assert.That(result, Is.Null);
    }
}
