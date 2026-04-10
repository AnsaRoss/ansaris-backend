using ErpApp.Application.Dtos.Products;
using ErpApp.Application.UseCases.Products;
using ErpApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;



namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ICreateProductUseCase _createProductUseCase;
        private readonly IGetAllProductsUseCase _getAllProductsUseCase;
        private readonly IGetProductByIdUseCase _getProductByIdUseCase;

        public ProductsController(
            ICreateProductUseCase createProductUseCase, 
            IGetAllProductsUseCase getAllProductsUseCase,
            IGetProductByIdUseCase getProductByIdUseCase)
        {
            _createProductUseCase = createProductUseCase;
            _getAllProductsUseCase = getAllProductsUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto productDto)
        {
            var createdProduct = await _createProductUseCase.Execute(productDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _getProductByIdUseCase.Execute(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _getAllProductsUseCase.ExecuteAsync();
            return Ok(products);
        }
    }
}