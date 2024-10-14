using AutoMapper;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Data.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<int> InsertProductAsync(ProductDTO product);
        Task UpdateProductAsync(ProductDTO product);
        Task DeleteProductAsync(int productId);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Method to return All products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        //Method to Find a Product By Id
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        //Method to Add a New Product and return the created Product ID
        public async Task<int> InsertProductAsync(ProductDTO product)
        {
            Product newProduct = _mapper.Map<Product>(product);
            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            product.ProductId = _context.Products.Count() + 1;
            return product.ProductId;
        }

        //Method to Update an Existing Customer
        public async Task UpdateProductAsync(ProductDTO product)
        {
            var existingProduct = await GetProductByIdAsync(product.ProductId);
            product.ProductId = existingProduct.ProductId;
            _mapper.Map(product, existingProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

    }

}
