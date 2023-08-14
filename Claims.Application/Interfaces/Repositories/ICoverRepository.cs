using Claims.Domain.Entities;

namespace Claims.Application.Interfaces.Repositories
{
    public interface ICoverRepository
    {
        Task<IEnumerable<Cover>> GetAllAsync();
        Task<Cover> GetByIdAsync(string id);
        Task AddAsync(Cover cover);
        Task DeleteAsync(string id);
    }
}
