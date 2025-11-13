namespace BackEndAPI.DTOs;

using System.ComponentModel.DataAnnotations;

public class SellerDTO
{
    [Required(ErrorMessage = "Name of seller is required.")]
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}