using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;

namespace Claims.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly CosmosDbService<Claim> _service;

        public ClaimRepository(CosmosDbService<Claim> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _service.GetAllAsync<Claim>();
        }

        public async Task<Claim> GetByIdAsync(string id)
        {
            return await _service.GetByIdAsync<Claim>(id);
        }

        public async Task AddAsync(Claim claim)
        {
            await _service.AddAsync(claim);
        }

        public async Task DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
        }
    }

}
