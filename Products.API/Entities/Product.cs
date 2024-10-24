using System.ComponentModel.DataAnnotations;

namespace Products.API.Entities;

public class Product
{
    public int Id { get; set; }
    [StringLength(150)]
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    [StringLength(250)]
    public string? Description { get; set; } = null;
}
