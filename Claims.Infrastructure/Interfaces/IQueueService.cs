namespace Claims.Infrastructure.Interfaces
{
    public interface IQueueService
    {
        Task EnqueueAuditDataAsync(string claimId, string httpRequestType);
    }
}
