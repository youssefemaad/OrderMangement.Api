using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var products = await productRepo.GetAllAsync();
            return mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(productId);
            return product == null ? null : mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var product = mapper.Map<Product>(productDto);
            var createdProduct = await productRepo.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto?> UpdateProductAsync(int productId, ProductDto productDto)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var existingProduct = await productRepo.GetByIdAsync(productId);
            if (existingProduct == null)
                return null;

            mapper.Map(productDto, existingProduct);
            existingProduct.ProductId = productId;

            await productRepo.UpdateAsync(existingProduct);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ProductDto>(existingProduct);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(productId);
            if (product == null)
                return false;

            await productRepo.DeleteAsync(product);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(productId);
            return product != null && product.Stock >= quantity;
        }
    }
}
