namespace EventHubReader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    public class EventProcessor : IEventProcessor
    {
        private Stopwatch stopWatch;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Shutting Down Event Processor");
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine("EventProcessor started");
            this.stopWatch = new Stopwatch();
            this.stopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                dynamic eventBody = JsonConvert.DeserializeObject(Encoding.Default.GetString(eventData.GetBytes()));
                Console.WriteLine(
                    "Part.ID: {0}, Part.Key: {1}, Guid: {2}, Location: {3}, Measure Name: {4}, Unit of Measure: {5}, Value: {6}",
                    context.Lease.PartitionId, 
                    eventData.PartitionKey, 
                    eventBody.guid, 
                    eventBody.location, 
                    eventBody.measurename, 
                    eventBody.unitofmeasure, 
                    eventBody.value);
            }

            if (this.stopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.stopWatch.Restart();
            }
        }
    }
}
