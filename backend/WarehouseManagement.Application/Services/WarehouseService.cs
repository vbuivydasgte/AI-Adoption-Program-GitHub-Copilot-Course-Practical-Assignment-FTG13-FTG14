using AutoMapper;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public class WarehouseService : CrudServiceBase<Warehouse, WarehouseDto>, IWarehouseService
{
    public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
        : base(warehouseRepository, mapper)
    {
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        return await GetAllAsync();
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto)
    {
        return await CreateAsync(createWarehouseDto);
    }

    public async Task<WarehouseDto?> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateWarehouseDto)
    {
        return await UpdateAsync(id, updateWarehouseDto, ApplyUpdates);
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        return await DeleteAsync(id);
    }

    private static void ApplyUpdates(Warehouse existingEntity, UpdateWarehouseDto updateDto)
    {
        existingEntity.Name = updateDto.Name;
        existingEntity.Location = updateDto.Location;
        existingEntity.ModifiedAt = DateTime.UtcNow;
    }
}
