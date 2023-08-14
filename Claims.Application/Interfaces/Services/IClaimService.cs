using Claims.Application.DataTransferObjects;
using Claims.Domain.Entities;

namespace Claims.Application.Interfaces.Services
{
    public interface IClaimService
    {
        Task<IEnumerable<ClaimDataTransferObject>> GetAllAsync();
        Task<ClaimDataTransferObject> CreateAsync(ClaimDataTransferObject claim);
        Task DeleteAsync(string id);
        Task<ClaimDataTransferObject> GetByIdAsync(string id);
    }
}
