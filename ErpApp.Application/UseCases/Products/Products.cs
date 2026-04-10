using ErpApp.Domain.Entities;

namespace ErpApp.Application.UseCases.Products
{
    public interface IGetAllProductsUseCase
    {
        Task<IEnumerable<Product>> ExecuteAsync();
    }
}
