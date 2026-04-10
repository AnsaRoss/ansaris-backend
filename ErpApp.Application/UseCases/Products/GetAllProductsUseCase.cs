using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Products
{
    public class GetAllProductsUseCase : IGetAllProductsUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> ExecuteAsync()
        {
            return await _productRepository.GetAllAsync();
        }
    }
}