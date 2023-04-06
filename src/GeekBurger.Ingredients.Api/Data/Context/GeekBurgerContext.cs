using GeekBurger.Ingredients.Api.Models;
using MongoDB.Driver;

namespace GeekBurger.Ingredients.Api.Data.Context
{
    public class GeekBurgerContext
    {
        private readonly IMongoDatabase Database;

        public GeekBurgerContext(Configuration configuration)
        {
            var cliente = new MongoClient(configuration.MongoDb.Connection);

            Database = cliente.GetDatabase(configuration.MongoDb.Database);
        }

        public IMongoCollection<Product> Products => Database.GetCollection<Product>("products");
    }
}
