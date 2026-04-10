using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ErpApp.Application.Dtos.Products;

namespace ErpApp.Application.UseCases.Products
{
    public interface ICreateProductUseCase
    {
        Task<Product> Execute(ProductCreateDto productDto);
    }
    public class CreateProductUseCase : ICreateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public CreateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Execute(ProductCreateDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                SKU = productDto.SKU,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            return product;
        }

        
    }
}