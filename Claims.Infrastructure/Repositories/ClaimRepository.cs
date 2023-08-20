using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using Claims.Infrastructure.Interfaces;

namespace Claims.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly CosmosDbService<Claim> _service;

        public ClaimRepository(ICosmosDbServiceFactory serviceFactory)
        {
            _service = serviceFactory.Create<Claim>();
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<Claim> GetByIdAsync(string id)
        {
            return await _service.GetByIdAsync(id);
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
