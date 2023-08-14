using Azure.Storage.Queues;
using Claims.Infrastructure.Interfaces;
using Claims.Infrastructure.Models;
using Newtonsoft.Json;

namespace Claims.Infrastructure
{
    //this can be more generic but for sake of time it's just going to work for claim audit as a proof of concept
    public class QueueService : IQueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task EnqueueAuditDataAsync(string claimId, string httpRequestType)
        {
            var auditData = new AuditData
            {
                ClaimId = claimId,
                HttpRequestType = httpRequestType
            };

            var serializedData = JsonConvert.SerializeObject(auditData);
            await _queueClient.SendMessageAsync(serializedData);
        }
    }
}
