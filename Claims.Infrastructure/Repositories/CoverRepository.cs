using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using Claims.Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure.Repositories
{
    public class CoverRepository : ICoverRepository
    {
        private readonly CosmosDbService<Cover> _service;

        public CoverRepository(ICosmosDbServiceFactory serviceFactory)
        {
            _service = serviceFactory.Create<Cover>();
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        public async Task<Cover> GetByIdAsync(string id)
        {
            return await _service.GetByIdAsync(id);
        }

        public async Task AddAsync(Cover cover)
        {
            await _service.AddAsync(cover);
        }

        public async Task DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
        }
    }
}
