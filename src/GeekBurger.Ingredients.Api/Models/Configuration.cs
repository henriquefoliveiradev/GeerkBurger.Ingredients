namespace GeekBurger.Ingredients.Api.Models
{
    public class Configuration
    {
        public ServiceBus ServiceBus { get; set; }

        public string ProductResource { get; set; }

        public MongoDb MongoDb { get; set; }
    }

    public class ServiceBus
    {
        public Queue LabelLoader { get; set; }
        public Queue Product { get; set; }
    }

    public class Queue
    {
        public string ConnectionString { get; set; }

        public string Path { get; set; }

        public string SubscriptionName { get; set; }
    }

    public class MongoDb
    {
        public string Connection { get; set; }

        public string Database { get; set; }
    }
}
