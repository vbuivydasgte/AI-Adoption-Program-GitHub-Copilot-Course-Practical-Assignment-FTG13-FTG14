using WarehouseManagement.Application.DTOs.Warehouses;

namespace WarehouseManagement.Application.Interfaces;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
    Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
    Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto createWarehouseDto);
    Task<WarehouseDto?> UpdateWarehouseAsync(int id, UpdateWarehouseDto updateWarehouseDto);
    Task<bool> DeleteWarehouseAsync(int id);
}
