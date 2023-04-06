using GeekBurger.Ingredients.Api.Events.Interfaces;
using GeekBurger.Ingredients.Api.Models;
using GeekBurger.Ingredients.Api.Services.Interfaces;
using GeekBurger.Products.Contract;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api.Events
{
    public class ProductChanged : Event, IProductChanged
    {
        private SubscriptionClient _subscriptionClient;
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly IProductService _productService;
        private static List<Task> PendingCompleteTasks;

        public ProductChanged(Configuration configuration, IProductService productService)
        {
            _connectionString = configuration.ServiceBus.Product.ConnectionString;
            _topicName = configuration.ServiceBus.Product.Path;
            _subscriptionName = configuration.ServiceBus.Product.SubscriptionName;
            _productService = productService;
        }

        public async Task ProcessMessages()
        {
            try
            {
                PendingCompleteTasks = new List<Task>();

                _subscriptionClient = new SubscriptionClient(_connectionString, _topicName, _subscriptionName, ReceiveMode.PeekLock);

                var handlerOptions = new MessageHandlerOptions(ExceptionHandler) { AutoComplete = true, MaxConcurrentCalls = 3 };

                _subscriptionClient.RegisterMessageHandler(MessageHandler, handlerOptions);

                await Task.WhenAll(PendingCompleteTasks);
            }
            catch 
            {
            }
        }

        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            if (_subscriptionClient.IsClosedOrClosing)
                return;

            string messageBody = Encoding.UTF8.GetString(message.Body);

            TraceEvent(messageBody);

            var productChangedMessage = JsonConvert.DeserializeObject<ProductChangedMessage>(messageBody);

            switch (productChangedMessage.State)
            {
                case ProductState.Deleted:
                    {
                        await _productService.Remove(productChangedMessage.Product.ProductId);
                        break;
                    }
                case ProductState.Modified:
                    {
                        await _productService.Update(productChangedMessage.Product);
                        break;
                    }
                default:
                    break;
            }

            Task PendingCompleteTask;
            lock (PendingCompleteTasks)
            {
                PendingCompleteTasks.Add(_subscriptionClient.CompleteAsync(message.SystemProperties.LockToken));
                PendingCompleteTask = PendingCompleteTasks.LastOrDefault();
            }

            await PendingCompleteTask;

            PendingCompleteTasks.Remove(PendingCompleteTask);
        }
    }
}
