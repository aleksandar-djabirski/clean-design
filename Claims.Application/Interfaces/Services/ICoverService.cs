using Claims.Application.DataTransferObjects;
using Claims.Domain.Enums;

namespace Claims.Application.Interfaces.Services
{
    public interface ICoverService
    {
        Task<IEnumerable<CoverDataTransferObject>> GetAllAsync();
        Task<CoverDataTransferObject> CreateAsync(CoverDataTransferObject cover);
        Task DeleteAsync(string id);
        Task<CoverDataTransferObject> GetByIdAsync(string id);
        decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
    }
}
