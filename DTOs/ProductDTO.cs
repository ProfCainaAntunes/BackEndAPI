namespace BackEndAPI.DTOs;

using System.ComponentModel.DataAnnotations;

public class ProductDTO
{
    [Required(ErrorMessage = "Name of product is required.")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Category of product is required.")]
    public string Category { get; set; } = string.Empty;
    [Required(ErrorMessage = "Price of product is required.")]
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}