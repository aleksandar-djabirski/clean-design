using Claims.Application.Interfaces.Repositories;
using Claims.Infrastructure.Interfaces;

namespace Claims.Infrastructure.Repositories
{
    namespace Claims.Infrastructure.Repositories
    {
        public class ClaimAuditRepository : IClaimAuditRepository
        {
            private readonly IQueueService _queueService;

            public ClaimAuditRepository(IQueueService queueService)
            {
                _queueService = queueService;
            }

            public async Task AuditAsync(string claimId, string httpRequestType)
            {
                await _queueService.EnqueueAuditDataAsync(claimId, httpRequestType);
            }
        }
    }
}
