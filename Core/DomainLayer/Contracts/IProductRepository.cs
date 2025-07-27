using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
        Task<bool> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> productIds);
    }
}