using AnthonyShares.MessageModels;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnthonyShares.Receiver
{
    class ReceiverConsole
    {
        static string ConnectionString = "Endpoint=sb://anthonydemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sQf+VJHPxxH2/+PbGxUiSwoZIEZUVo1cwtZqJ5I8/Kg=";
        static string QueueName = "demoqueue";

        private static QueueClient QueueClient;

        static void Main(string[] args)
        {
            ReceiveAndProcessStocks(1);
        }

        static void ReceiveAndProcessStocks(int threads)
        {
            Console.WriteLine($"ReceiveAndProcessStocks({ threads })", ConsoleColor.Cyan);
            // Create a new client
            QueueClient = new QueueClient(ConnectionString, QueueName);

            // Set the options for the message handler
            var options = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = threads,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(10)
            };

            // Create a message pump using RegisterMessageHandler
            QueueClient.RegisterMessageHandler(ProcessStockMessageAsync, options);


            Console.WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();
            StopReceivingAsync().Wait();
        }

        static async Task ProcessStockMessageAsync(Message message, CancellationToken token)
        {

            // Deserialize the message body.
            var messageBodyText = Encoding.UTF8.GetString(message.Body);

            var stock = JsonConvert.DeserializeObject<Stock>(messageBodyText);

            // Process the message
            ProcessStock(stock);

            // Complete the message
            await QueueClient.CompleteAsync(message.SystemProperties.LockToken);

        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception.Message, ConsoleColor.Red);
            return Task.CompletedTask;
        }

        static async Task StopReceivingAsync()
        {
            // Close the client, which will stop the message pump.
            await QueueClient.CloseAsync();
        }

        private static void ProcessStock(Stock stock)
        {
            
        }
    }
}
