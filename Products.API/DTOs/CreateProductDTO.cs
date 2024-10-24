namespace Products.API.DTOs;

public class CreateProductDTO
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Description { get; set; } = default!;
}
