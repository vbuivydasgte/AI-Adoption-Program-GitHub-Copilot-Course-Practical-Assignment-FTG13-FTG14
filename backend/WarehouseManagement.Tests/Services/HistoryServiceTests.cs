using AutoMapper;
using NSubstitute;
using NUnit.Framework;
using WarehouseManagement.Application.DTOs.History;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Tests.Services;

[TestFixture]
public class HistoryServiceTests
{
    private IHistoryRepository _historyRepository = null!;
    private IMapper _mapper = null!;
    private HistoryService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _historyRepository = Substitute.For<IHistoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _service = new HistoryService(_historyRepository, _mapper);
    }

    [Test]
    public async Task GetStockHistoryAsync_ReturnsMappedHistory()
    {
        var historyEntities = new List<StockHistory>
        {
            new() { Id = 1, StockId = 10, ProductId = 2, WarehouseId = 4, ChangedBy = "worker", ChangedAt = DateTime.UtcNow, PreviousQuantity = 1, NewQuantity = 3, QuantityChange = 2, Reason = "Receive" }
        };

        var mappedHistory = new List<StockHistoryDto>
        {
            new() { Id = 1, StockId = 10, ProductId = 2, WarehouseId = 4, ProductName = "Laptop", WarehouseName = "Main", ChangedBy = "worker", ChangedAt = DateTime.UtcNow, PreviousQuantity = 1, NewQuantity = 3, QuantityChange = 2, Reason = "Receive" }
        };

        _historyRepository.GetStockHistoryAsync(10).Returns(historyEntities);
        _mapper.Map<List<StockHistoryDto>>(historyEntities).Returns(mappedHistory);

        var result = (await _service.GetStockHistoryAsync(10)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ProductName, Is.EqualTo("Laptop"));
        Assert.That(result[0].WarehouseName, Is.EqualTo("Main"));
    }

    [Test]
    public async Task GetStockHistoryByWarehouseAsync_ReturnsMappedHistory()
    {
        var historyEntities = new List<StockHistory>
        {
            new() { Id = 3, StockId = 12, ProductId = 7, WarehouseId = 8, ChangedBy = "admin", ChangedAt = DateTime.UtcNow, PreviousQuantity = 5, NewQuantity = 6, QuantityChange = 1, Reason = "Adjustment" }
        };

        var mappedHistory = new List<StockHistoryDto>
        {
            new() { Id = 3, StockId = 12, ProductId = 7, WarehouseId = 8, ProductName = "Keyboard", WarehouseName = "Overflow", ChangedBy = "admin", ChangedAt = DateTime.UtcNow, PreviousQuantity = 5, NewQuantity = 6, QuantityChange = 1, Reason = "Adjustment" }
        };

        _historyRepository.GetStockHistoryByWarehouseAsync(8).Returns(historyEntities);
        _mapper.Map<List<StockHistoryDto>>(historyEntities).Returns(mappedHistory);

        var result = (await _service.GetStockHistoryByWarehouseAsync(8)).ToList();

        Assert.That(result[0].ProductName, Is.EqualTo("Keyboard"));
        Assert.That(result[0].WarehouseName, Is.EqualTo("Overflow"));
    }

    [Test]
    public async Task GetStockHistoryByProductAsync_ReturnsMappedHistory()
    {
        var historyEntities = new List<StockHistory>
        {
            new() { Id = 5, StockId = 90, ProductId = 13, WarehouseId = 14, ChangedBy = "user", ChangedAt = DateTime.UtcNow, PreviousQuantity = 2, NewQuantity = 9, QuantityChange = 7, Reason = "Restock" }
        };

        var mappedHistory = new List<StockHistoryDto>
        {
            new() { Id = 5, StockId = 90, ProductId = 13, WarehouseId = 14, ProductName = "Monitor", WarehouseName = "Secondary", ChangedBy = "user", ChangedAt = DateTime.UtcNow, PreviousQuantity = 2, NewQuantity = 9, QuantityChange = 7, Reason = "Restock" }
        };

        _historyRepository.GetStockHistoryByProductAsync(13).Returns(historyEntities);
        _mapper.Map<List<StockHistoryDto>>(historyEntities).Returns(mappedHistory);

        var result = (await _service.GetStockHistoryByProductAsync(13)).ToList();

        Assert.That(result[0].ProductName, Is.EqualTo("Monitor"));
        Assert.That(result[0].WarehouseName, Is.EqualTo("Secondary"));
    }

    [Test]
    public async Task GetStockHistoryAsync_WhenNoRows_ReturnsEmptyList()
    {
        var historyEntities = new List<StockHistory>();
        var mappedHistory = new List<StockHistoryDto>();

        _historyRepository.GetStockHistoryAsync(999).Returns(historyEntities);
        _mapper.Map<List<StockHistoryDto>>(historyEntities).Returns(mappedHistory);

        var result = (await _service.GetStockHistoryAsync(999)).ToList();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllStockHistoryAsync_ReturnsMappedHistory()
    {
        var historyEntities = new List<StockHistory>
        {
            new() { Id = 7, StockId = 22, ProductId = 3, WarehouseId = 5, ChangedBy = "admin", ChangedAt = DateTime.UtcNow, PreviousQuantity = 10, NewQuantity = 12, QuantityChange = 2, Reason = "Correction" }
        };

        var mappedHistory = new List<StockHistoryDto>
        {
            new() { Id = 7, StockId = 22, ProductId = 3, WarehouseId = 5, ProductName = "Mouse", WarehouseName = "Primary", ChangedBy = "admin", ChangedAt = DateTime.UtcNow, PreviousQuantity = 10, NewQuantity = 12, QuantityChange = 2, Reason = "Correction" }
        };

        _historyRepository.GetAllStockHistoryAsync().Returns(historyEntities);
        _mapper.Map<List<StockHistoryDto>>(historyEntities).Returns(mappedHistory);

        var result = (await _service.GetAllStockHistoryAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ProductName, Is.EqualTo("Mouse"));
        Assert.That(result[0].WarehouseName, Is.EqualTo("Primary"));
    }
}
