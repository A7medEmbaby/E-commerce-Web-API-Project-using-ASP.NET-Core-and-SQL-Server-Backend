using AutoMapper;
using ECommerceAPI.Data.Repositories;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _imapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _imapper = mapper;
        }

        // GET: api/product
        [HttpGet("All")]
        public async Task<APIResponse<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                return new APIResponse<List<Product>>(products, "Retrieved all products successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<Product>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/product/5
        [HttpGet("ById/{id}")]
        public async Task<APIResponse<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<Product>(HttpStatusCode.NotFound, "Product not found.");
                }
                return new APIResponse<Product>(product, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Product>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/product
        [HttpPost("Create")]
        public async Task<APIResponse<ProductResponseDTO>> CreateProduct([FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<ProductResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var productId = await _productRepository.InsertProductAsync(product);
                var responseDTO = _imapper.Map<ProductResponseDTO>(product);
                return new APIResponse<ProductResponseDTO>(responseDTO, "Product created successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<ProductResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/product/5
        [HttpPut("{id}/Update")]
        public async Task<APIResponse<bool>> UpdateProduct(int id, [FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            if (id != product.ProductId)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Mismatched product ID");
            }

            try
            {
                var existingProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
                if (existingProduct == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Product not found.");
                }

                await _productRepository.UpdateProductAsync(product);
                return new APIResponse<bool>(true, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // DELETE: api/product/5
        [HttpDelete("{id}/Delete")]
        public async Task<APIResponse<bool>> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Product not found.");
                }

                await _productRepository.DeleteProductAsync(id);
                return new APIResponse<bool>(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }


    }
}
