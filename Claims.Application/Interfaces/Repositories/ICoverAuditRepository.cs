namespace Claims.Application.Interfaces.Repositories
{
    public interface ICoverAuditRepository
    {
        void Audit(string coverId, string httpRequestType);
    }
}
