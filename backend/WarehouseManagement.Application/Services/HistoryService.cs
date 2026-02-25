using AutoMapper;
using WarehouseManagement.Application.DTOs.History;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IMapper _mapper;

    public HistoryService(
        IHistoryRepository historyRepository,
        IMapper mapper)
    {
        _historyRepository = historyRepository;
        _mapper = mapper;
    }

    public Task<IEnumerable<StockHistoryDto>> GetStockHistoryAsync(int stockId)
        => GetMappedHistoryAsync(() => _historyRepository.GetStockHistoryAsync(stockId));

    public Task<IEnumerable<StockHistoryDto>> GetAllStockHistoryAsync()
        => GetMappedHistoryAsync(_historyRepository.GetAllStockHistoryAsync);

    public Task<IEnumerable<StockHistoryDto>> GetStockHistoryByProductAsync(int productId)
        => GetMappedHistoryAsync(() => _historyRepository.GetStockHistoryByProductAsync(productId));

    public Task<IEnumerable<StockHistoryDto>> GetStockHistoryByWarehouseAsync(int warehouseId)
        => GetMappedHistoryAsync(() => _historyRepository.GetStockHistoryByWarehouseAsync(warehouseId));

    private async Task<IEnumerable<StockHistoryDto>> GetMappedHistoryAsync(
        Func<Task<IEnumerable<StockHistory>>> fetchHistory)
    {
        var history = await fetchHistory();
        return _mapper.Map<List<StockHistoryDto>>(history);
    }
}
