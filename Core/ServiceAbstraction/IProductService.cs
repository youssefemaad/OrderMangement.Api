using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<ProductDto?> UpdateProductAsync(int productId, ProductDto productDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
    }
}
