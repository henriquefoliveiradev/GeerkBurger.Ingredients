using GeekBurger.Ingredients.Api.Data.Context;
using GeekBurger.Ingredients.Api.Data.Intefaces;
using GeekBurger.Ingredients.Api.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly GeekBurgerContext _context;

        public ProductRepository(GeekBurgerContext context)
        {
            _context = context;
        }

        public async Task<Product> Get(Guid productId)
        {
            var result = await _context.Products.FindAsync(p => p.ProductId == productId);

            return await result.FirstOrDefaultAsync();
        }

        public async Task Save(Product product)
        {
            var options = new UpdateOptions() { IsUpsert = true };

            await _context.Products
                .ReplaceOneAsync(p => p.ProductId == product.ProductId, product, options);
        }

        public async Task<IEnumerable<Product>> Get(IEnumerable<string> restrictions)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Empty;

            //if (restrictions != null && restrictions.Any())
            //{
            //    filter = Builders<Product>.Filter.In("items.ingredients", restrictions);
            //}

            var result = await _context.Products.FindAsync(filter);

            var products = await result.ToListAsync();

            // TODO: Improvement filter in memory
            if (restrictions != null && restrictions.Any())
                return products.Where(p => p.Items.Any(i => !i.Ingredients.Any(g => restrictions.Contains(g.ToLower(), StringComparer.InvariantCultureIgnoreCase))));

            return products;
        }

        public async Task Remove(Guid productId)
        {
            await _context.Products.DeleteOneAsync(p => p.ProductId == productId);
        }
    }
}
