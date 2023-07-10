using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

class Program
{
    static async Task Main()
    {

        string connectionString = "Endpoint=sb://cignium-cds-prod-ns.servicebus.windows.net/;SharedAccessKeyName=Reader;SharedAccessKey=wHCwlN03v7v4Atr1dytq1XXLCFT1xJsKp11MzXvAPqA=";
        string topicName = "agent-reporting-topic";
        string subscriptionName = "integration";

        // Create a ServiceBusClient
        await using var client = new ServiceBusClient(connectionString);

        // Create a ServiceBusProcessor for the subscription
        ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

        // Register the message handler
        processor.ProcessMessageAsync += ProcessMessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Start processing messages
        await processor.StartProcessingAsync();

        Console.WriteLine("Receiving messages... Press any key to stop.");

        // Wait for a key press to stop processing
        Console.ReadKey();

        // Stop processing messages and close the processor
        await processor.StopProcessingAsync();
        await processor.DisposeAsync();
    }

    static async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        // Retrieve the message body as a string
        string body = args.Message.Body.ToString();

        // Process the message
        Console.WriteLine($"Received message: SequenceNumber={args.Message.SequenceNumber} Body={body}");

        // Complete the message to remove it from the subscription
        await args.CompleteMessageAsync(args.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // Handle any errors that occur during message processing
        Console.WriteLine($"Error occurred: {args.Exception}");

        return Task.CompletedTask;
    }
}
