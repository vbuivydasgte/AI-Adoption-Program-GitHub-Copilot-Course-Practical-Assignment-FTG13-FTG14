using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Application.DTOs.Products;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "SKU must be between 1 and 50 characters")]
    public string SKU { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}
