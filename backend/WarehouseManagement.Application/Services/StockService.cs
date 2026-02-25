using AutoMapper;
using WarehouseManagement.Application.Common;
using WarehouseManagement.Application.DTOs.Stock;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Application.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly IHistoryRepository _historyRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public StockService(
        IStockRepository stockRepository,
        IHistoryRepository historyRepository,
        ApplicationDbContext dbContext,
        IMapper mapper)
    {
        _stockRepository = stockRepository;
        _historyRepository = historyRepository;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StockDto>> GetAllStockAsync()
    {
        var stocks = await _stockRepository.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<StockDto>>(stocks);
    }

    public async Task<IEnumerable<StockDto>> GetStockByWarehouseAsync(int warehouseId)
    {
        var stocks = await _stockRepository.GetByWarehouseIdAsync(warehouseId);
        return _mapper.Map<IEnumerable<StockDto>>(stocks);
    }

    public async Task<IEnumerable<StockDto>> GetStockByProductAsync(int productId)
    {
        var stocks = await _stockRepository.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<StockDto>>(stocks);
    }

    public async Task<StockDto?> GetStockByIdAsync(int id)
    {
        var stock = await _stockRepository.GetByIdWithDetailsAsync(id);
        return stock == null ? null : _mapper.Map<StockDto>(stock);
    }

    public async Task<StockDto> AdjustStockAsync(StockAdjustmentDto adjustmentDto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var stock = await _stockRepository.GetByProductAndWarehouseAsync(
                adjustmentDto.ProductId,
                adjustmentDto.WarehouseId
            );

            if (stock == null)
            {
                EnsureNonNegativeQuantity(adjustmentDto.QuantityChange);

                // Create new stock record
                stock = new Stock
                {
                    ProductId = adjustmentDto.ProductId,
                    WarehouseId = adjustmentDto.WarehouseId,
                    Quantity = adjustmentDto.QuantityChange,
                    LastUpdated = DateTime.UtcNow
                };
                stock = await _stockRepository.AddAsync(stock);

                // Add history for new stock
                await _historyRepository.AddStockHistoryAsync(CreateStockHistory(stock.Id, adjustmentDto, 0, stock.Quantity));
            }
            else
            {
                // Update existing stock
                var previousQuantity = stock.Quantity;
                var newQuantity = previousQuantity + adjustmentDto.QuantityChange;
                EnsureNonNegativeQuantity(newQuantity);

                stock.Quantity = newQuantity;
                stock.LastUpdated = DateTime.UtcNow;

                await _stockRepository.UpdateAsync(stock);

                // Add history
                await _historyRepository.AddStockHistoryAsync(CreateStockHistory(stock.Id, adjustmentDto, previousQuantity, stock.Quantity));
            }

            await transaction.CommitAsync();

            var updatedStock = await _stockRepository.GetByIdWithDetailsAsync(stock.Id);
            return _mapper.Map<StockDto>(updatedStock);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static void EnsureNonNegativeQuantity(int quantity)
    {
        if (quantity < 0)
        {
            throw new InvalidOperationException(ValidationMessages.StockQuantityCannotBeNegative);
        }
    }

    private static StockHistory CreateStockHistory(
        int stockId,
        StockAdjustmentDto adjustmentDto,
        int previousQuantity,
        int newQuantity)
    {
        return new StockHistory
        {
            StockId = stockId,
            ProductId = adjustmentDto.ProductId,
            WarehouseId = adjustmentDto.WarehouseId,
            ChangedBy = adjustmentDto.ChangedBy,
            ChangedAt = DateTime.UtcNow,
            PreviousQuantity = previousQuantity,
            NewQuantity = newQuantity,
            QuantityChange = adjustmentDto.QuantityChange,
            Reason = adjustmentDto.Reason
        };
    }
}
