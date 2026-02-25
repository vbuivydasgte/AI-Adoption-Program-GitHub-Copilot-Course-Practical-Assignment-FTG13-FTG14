using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Application.DTOs.Stock;

public class StockAdjustmentDto
{
    [Required(ErrorMessage = "Product ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Warehouse ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Warehouse ID must be greater than 0")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "Quantity change is required")]
    [Range(-10000, 10000, ErrorMessage = "Quantity change must be between -10000 and 10000")]
    public int QuantityChange { get; set; }

    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Reason must be between 1 and 500 characters")]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "Changed by is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Changed by must be between 1 and 100 characters")]
    public string ChangedBy { get; set; } = string.Empty;
}
