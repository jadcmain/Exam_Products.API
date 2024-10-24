using Microsoft.EntityFrameworkCore;
using Products.API.Entities;

namespace Products.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext context;

    public ProductRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<int> Create(Product product)
    {
        await context.AddAsync<Product>(product);
        await context.SaveChangesAsync();
        return product.Id;
    }

    public async Task Delete(int id)
    {
        await context.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await context.Products.AnyAsync(x => x.Id == id);
    }

    public async Task<List<Product>> GetAll()
    {
        return await context.Products.ToListAsync();
    }

    public async Task<Product?> GetById(int id)
    {
        return await context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(Product product)
    {
        context.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task<bool> NameExists(string name, int id)
    {
        return await context.Products.AnyAsync(x => x.Name == name && x.Id != id);
    }
}
