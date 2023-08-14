using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using Claims.Infrastructure.Data.DbContexts;

namespace Claims.Infrastructure.Repositories
{
    public class CoverAuditRepository : ICoverAuditRepository
    {
        private readonly AuditDbContext _dbContext;

        public CoverAuditRepository(AuditDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Audit(string coverId, string httpRequestType)
        {
            var audit = new CoverAudit
            {
                CoverId = coverId,
                HttpRequestType = httpRequestType,
                Created = DateTime.Now
            };

            _dbContext.CoverAudits.Add(audit);
            _dbContext.SaveChanges();
        }
    }
}
