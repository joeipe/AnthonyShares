using AnthonyShares.MessageModels;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AnthonyShares.Sender
{
    class SenderConsole
    {
        static string ConnectionString = "";
        static string QueueName = "demoqueue";

        static void Main(string[] args)
        {
            SendStocksAsync().Wait();
        }

        static async Task SendStocksAsync()
        {
            var stock = new Stock()
            {
                Name = "VCX",
            };

            // Serialize the order object
            var jsonStock = JsonConvert.SerializeObject(stock);

            // Create a Message
            var message = new Message(Encoding.UTF8.GetBytes(jsonStock))
            {
                Label = "Stock",
                ContentType = "application/json"
            };

            // Send the message...
            var client = new QueueClient(ConnectionString, QueueName);
            Console.Write("Sending order...", ConsoleColor.Green);
            await client.SendAsync(message);
            Console.WriteLine("Done!", ConsoleColor.Green);
            Console.WriteLine();
            await client.CloseAsync();
        }
    }
}
