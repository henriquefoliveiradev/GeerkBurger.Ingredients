using GeekBurger.Ingredients.Api.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.SendMessage
{
    class Program
    {
        private static IList<Message> _messages = new List<Message>();
        private static Task _lastTask;

        static void Main(string[] args)
        {
            SendMessage().Wait();
        }

        public static async Task SendMessage()
        {
            if (_lastTask != null && !_lastTask.IsCompleted)
                return;

            _messages.Add(CreateMessage());

            var connectionString = "Endpoint=sb://geekburger-ingredients.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Bk416xr+T91RlSN/xdM9tjCyo9R0tWRu1Fceb3qpJmc=";
            var queueName = "label";
            var queueClient = new QueueClient(connectionString, queueName);

            _lastTask = Send(queueClient);

            await _lastTask;

            var closeTask = queueClient.CloseAsync();
            await closeTask;

            HandleException(closeTask);
        }

        private static async Task Send(QueueClient queueClient)
        {
            int tries = 0;
            Message message;

            while (true)
            {
                if (_messages.Count <= 0)
                    break;

                lock (_messages)
                    message = _messages.FirstOrDefault();

                var sendTask = queueClient.SendAsync(message);
                await sendTask;

                var success = HandleException(sendTask);

                if (!success)
                    Thread.Sleep(10000 * (tries < 60 ? tries++ : tries));
                else
                    _messages.Remove(message);
            }
        }

        private static Message CreateMessage()
        {
            var label = new Label
            {
                ItemName = "Bacon Burger",
                Ingredients = new List<string>
                {
                    "Picles",
                    "Meat Burger",
                    "Smoked Bacon"
                }
            };

            string labelSerialized = JsonConvert.SerializeObject(label);

            byte[] labelByteArray = Encoding.UTF8.GetBytes(labelSerialized);

            return new Message
            {
                Body = labelByteArray,
                MessageId = Guid.NewGuid().ToString(),
                Label = label.ItemName
            };
        }

        private static bool HandleException(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList().ForEach(innerException =>
            {
                Console.WriteLine($"Error in SendAsync task: {innerException.Message}. Details:{innerException.StackTrace} ");

                if (innerException is ServiceBusCommunicationException)
                    Console.WriteLine("Connection Problem with Host. Internet Connection can be down");
            });

            return false;
        }
    }
}
