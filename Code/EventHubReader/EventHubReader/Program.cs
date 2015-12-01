namespace EventHubReader
{
    using System;
    using Microsoft.ServiceBus.Messaging;

    public class Program
    {
        public static void Main(string[] args)
        {
            string eventHubConnectionString = "Endpoint=sb://[EventHubNamespaceName].servicebus.windows.net/;SharedAccessKeyName=[SASKeyName];SharedAccessKey=[SASKey]";
            string eventHubName = "[EventHubName]";
            string storageAccountName = "[StorageAccountName]";
            string storageAccountKey = "[StorageAccountKey]";
            string storageConnectionString = string.Format(
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                storageAccountName, 
                storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering the EventProcessor");
            eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>().Wait();

            Console.WriteLine("Listening... Press enter to stop.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
