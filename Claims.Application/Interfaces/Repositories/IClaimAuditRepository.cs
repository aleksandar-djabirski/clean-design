namespace Claims.Application.Interfaces.Repositories
{
    public interface IClaimAuditRepository
    {
        Task AuditAsync(string claimId, string httpRequestType);
    }
}
