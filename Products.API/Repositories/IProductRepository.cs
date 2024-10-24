using Products.API.Entities;

namespace Products.API.Repositories;

public interface IProductRepository
{
    Task<int> Create(Product product);
    Task<Product?> GetById(int id);
    Task<List<Product>> GetAll();
    Task<bool> Exists(int id);
    Task Update(Product product);
    Task Delete(int id);
    Task<bool> NameExists(string name, int id);
}
