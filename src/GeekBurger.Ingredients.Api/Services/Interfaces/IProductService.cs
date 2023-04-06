using GeekBurger.Ingredients.Api.Models;
using GeekBurger.Products.Contract;
using System;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task AddIngredients(Label label);

        Task Update(ProductToGet productToGet);

        Task Remove(Guid productId);
    }
}
