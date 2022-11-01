using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace Infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;
        private readonly IElasticClient _elasticClient;
        public ProductRepository(IElasticClient elasticClient, StoreContext context)
        {
            _context = context;
            _elasticClient = elasticClient;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            return await _context.Products.Where(p => p.Id ==1).Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsElastic(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Product>(
                        s => s.Query(
                            q => q.QueryString(
                                d => d.Query('*' + keyword + '*')
                            )).Size(5000));

            return result.Documents.ToList();
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _context.ProductBrands.ToListAsync();
        }

    }
}