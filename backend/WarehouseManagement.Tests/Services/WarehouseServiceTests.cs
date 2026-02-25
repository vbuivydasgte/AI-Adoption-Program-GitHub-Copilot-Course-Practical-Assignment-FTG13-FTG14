using AutoMapper;
using NSubstitute;
using NUnit.Framework;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Tests.Services;

[TestFixture]
public class WarehouseServiceTests
{
    private IWarehouseRepository _warehouseRepository = null!;
    private IMapper _mapper = null!;
    private WarehouseService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _warehouseRepository = Substitute.For<IWarehouseRepository>();
        _mapper = Substitute.For<IMapper>();
        _service = new WarehouseService(_warehouseRepository, _mapper);
    }

    [Test]
    public async Task GetAllWarehousesAsync_ReturnsMappedWarehouses()
    {
        var warehouses = new List<Warehouse>
        {
            new() { Id = 1, Name = "Main", Location = "Vilnius" },
            new() { Id = 2, Name = "Secondary", Location = "Kaunas" }
        };

        var mapped = new List<WarehouseDto>
        {
            new() { Id = 1, Name = "Main", Location = "Vilnius" },
            new() { Id = 2, Name = "Secondary", Location = "Kaunas" }
        };

        _warehouseRepository.GetAllAsync().Returns(warehouses);
        _mapper.Map<IEnumerable<WarehouseDto>>(warehouses).Returns(mapped);

        var result = (await _service.GetAllWarehousesAsync()).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Main"));
    }

    [Test]
    public async Task GetWarehouseByIdAsync_WhenFound_ReturnsMappedWarehouse()
    {
        var warehouse = new Warehouse { Id = 5, Name = "Central", Location = "Riga" };
        var mapped = new WarehouseDto { Id = 5, Name = "Central", Location = "Riga" };

        _warehouseRepository.GetByIdAsync(5).Returns(warehouse);
        _mapper.Map<WarehouseDto>(warehouse).Returns(mapped);

        var result = await _service.GetWarehouseByIdAsync(5);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Location, Is.EqualTo("Riga"));
    }

    [Test]
    public async Task GetWarehouseByIdAsync_WhenMissing_ReturnsNull()
    {
        _warehouseRepository.GetByIdAsync(50).Returns((Warehouse?)null);

        var result = await _service.GetWarehouseByIdAsync(50);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateWarehouseAsync_CreatesAndReturnsMappedWarehouse()
    {
        var createDto = new CreateWarehouseDto { Name = "North", Location = "Tallinn" };
        var mappedWarehouse = new Warehouse { Name = "North", Location = "Tallinn" };
        var createdWarehouse = new Warehouse { Id = 6, Name = "North", Location = "Tallinn" };
        var mappedDto = new WarehouseDto { Id = 6, Name = "North", Location = "Tallinn" };

        _mapper.Map<Warehouse>(createDto).Returns(mappedWarehouse);
        _warehouseRepository.AddAsync(mappedWarehouse).Returns(createdWarehouse);
        _mapper.Map<WarehouseDto>(createdWarehouse).Returns(mappedDto);

        var result = await _service.CreateWarehouseAsync(createDto);

        Assert.That(result.Id, Is.EqualTo(6));
        await _warehouseRepository.Received(1).AddAsync(mappedWarehouse);
    }

    [Test]
    public async Task UpdateWarehouseAsync_WhenMissing_ReturnsNull()
    {
        var updateDto = new UpdateWarehouseDto { Name = "Updated", Location = "City" };
        _warehouseRepository.GetByIdAsync(77).Returns((Warehouse?)null);

        var result = await _service.UpdateWarehouseAsync(77, updateDto);

        Assert.That(result, Is.Null);
        await _warehouseRepository.DidNotReceive().UpdateAsync(Arg.Any<Warehouse>());
    }

    [Test]
    public async Task UpdateWarehouseAsync_WhenFound_UpdatesAndReturnsMapped()
    {
        var existing = new Warehouse
        {
            Id = 9,
            Name = "Old",
            Location = "OldLoc",
            ModifiedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updateDto = new UpdateWarehouseDto { Name = "New", Location = "NewLoc" };
        var mappedDto = new WarehouseDto { Id = 9, Name = "New", Location = "NewLoc" };

        _warehouseRepository.GetByIdAsync(9).Returns(existing);
        _mapper.Map<WarehouseDto>(existing).Returns(mappedDto);

        var result = await _service.UpdateWarehouseAsync(9, updateDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("New"));
        await _warehouseRepository.Received(1).UpdateAsync(Arg.Is<Warehouse>(w =>
            w.Id == 9 && w.Name == "New" && w.Location == "NewLoc"));
    }

    [Test]
    public async Task DeleteWarehouseAsync_WhenMissing_ReturnsFalse()
    {
        _warehouseRepository.GetByIdAsync(12).Returns((Warehouse?)null);

        var result = await _service.DeleteWarehouseAsync(12);

        Assert.That(result, Is.False);
        await _warehouseRepository.DidNotReceive().DeleteAsync(Arg.Any<Warehouse>());
    }

    [Test]
    public async Task DeleteWarehouseAsync_WhenFound_DeletesAndReturnsTrue()
    {
        var warehouse = new Warehouse { Id = 12, Name = "Delete", Location = "Here" };
        _warehouseRepository.GetByIdAsync(12).Returns(warehouse);

        var result = await _service.DeleteWarehouseAsync(12);

        Assert.That(result, Is.True);
        await _warehouseRepository.Received(1).DeleteAsync(warehouse);
    }
}
