using GeekBurger.Ingredients.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Data.Intefaces
{
    public interface IProductRepository
    {
        Task<Product> Get(Guid productId);

        Task<IEnumerable<Product>> Get(IEnumerable<string> restrictions);

        Task Save(Product product);

        Task Remove(Guid productId);
    }
}
