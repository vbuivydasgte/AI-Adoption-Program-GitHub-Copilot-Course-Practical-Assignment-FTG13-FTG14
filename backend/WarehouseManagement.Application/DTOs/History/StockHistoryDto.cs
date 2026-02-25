namespace WarehouseManagement.Application.DTOs.History;

public class StockHistoryDto
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int QuantityChange { get; set; }
    public string Reason { get; set; } = string.Empty;
}
