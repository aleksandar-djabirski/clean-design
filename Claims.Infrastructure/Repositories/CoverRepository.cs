using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure.Repositories
{
    public class CoverRepository : ICoverRepository
    {
        private readonly CosmosDbService<Cover> _service;

        public CoverRepository(CosmosDbService<Cover> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _service.GetAllAsync<Cover>();
        }

        public async Task<Cover> GetByIdAsync(string id)
        {
            return await _service.GetByIdAsync<Cover>(id);
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
