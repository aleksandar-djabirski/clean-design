using Claims.Domain.Entities;
using Claims.Infrastructure.Data.DbContexts;
using Claims.Infrastructure.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Claims.Azure.Functions
{
    public static class AuditFunction
    {
        private static readonly string AuditDbConnectionString = Environment.GetEnvironmentVariable("AuditDbConnectionString");

        [Function("ProcessAudit")]
        public static async Task Run(
            [QueueTrigger("claim-audit", Connection = "AzureWebJobsStorage")] string queueItem,
            ILogger log)
        {
            var auditData = JsonConvert.DeserializeObject<AuditData>(queueItem);

            var options = new DbContextOptionsBuilder<AuditDbContext>()
               .UseSqlServer(AuditDbConnectionString) // Replace with your connection string
               .Options;

            using (var dbContext = new AuditDbContext(options))
            {
                var audit = new ClaimAudit
                {
                    ClaimId = auditData.ClaimId,
                    HttpRequestType = auditData.HttpRequestType,
                    Created = DateTime.Now
                };

                dbContext.ClaimAudits.Add(audit);
                await dbContext.SaveChangesAsync();
            }

            log.LogInformation($"Audit completed for Claim ID: {auditData.ClaimId}");
        }
    }
}
