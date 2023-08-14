using AutoMapper;
using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Repositories;
using Claims.Application.Interfaces.Services;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using Claims.Domain.Validation;
using FluentValidation;

namespace Claims.Application.Services
{
    public class CoverService : ICoverService
    {
        private readonly ICoverRepository _coverRepository;
        private readonly ICoverAuditRepository _coverAuditRepository;
        private readonly IMapper _mapper;
        private readonly CoverValidator _coverValidator;

        public CoverService(ICoverRepository coverRepository, ICoverAuditRepository coverAuditRepository, IMapper mapper, CoverValidator coverValidator)
        {
            _coverRepository = coverRepository ?? throw new ArgumentNullException(nameof(coverRepository));
            _coverAuditRepository = coverAuditRepository ?? throw new ArgumentNullException(nameof(coverAuditRepository));
            _mapper = mapper;
            _coverValidator = coverValidator;
        }

        public async Task<IEnumerable<CoverDataTransferObject>> GetAllAsync()
        {
            var covers = await _coverRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CoverDataTransferObject>>(covers);
        }

        public async Task<CoverDataTransferObject> GetByIdAsync(string id)
        {
            var cover = await _coverRepository.GetByIdAsync(id);
            return _mapper.Map<CoverDataTransferObject>(cover);
        }

        public async Task<CoverDataTransferObject> CreateAsync(CoverDataTransferObject coverDto)
        {
            var cover = _mapper.Map<Cover>(coverDto);
            cover.Id = Guid.NewGuid().ToString();

            var validationResult = _coverValidator.Validate(cover);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _coverRepository.AddAsync(cover);
            _coverAuditRepository.Audit(cover.Id, "POST");
            return _mapper.Map<CoverDataTransferObject>(cover);
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _coverRepository.DeleteAsync(id);
            _coverAuditRepository.Audit(id, "DELETE");
        }

        public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
        {
            var baseDayRate = 1250m;
            var multiplier = GetMultiplierBasedOnCoverType(coverType);

            var totalDays = (new DateTime(endDate.Year, endDate.Month, endDate.Day) - new DateTime(startDate.Year, startDate.Month, startDate.Day)).Days;
            var totalPremium = 0m;

            for (int i = 1; i <= totalDays; i++)
            {
                // First 30 days are computed based on the logic below
                var currentDayRate = baseDayRate * multiplier;

                if (i > 30 && i <= 180)
                {
                    // Following 150 days are discounted by 5% for Yacht and by 2% for other types
                    currentDayRate *= coverType == CoverType.Yacht ? 0.95m : 0.98m;
                }
                else if (i > 180)
                {
                    // The remaining days are discounted by additional 3% for Yacht and by 1% for other types
                    currentDayRate *= coverType == CoverType.Yacht ? 0.92m : 0.97m; // Note: The 5% discount is cumulative with the 3%.
                }

                totalPremium += currentDayRate;
            }

            return totalPremium;
        }

        // The switch can be replaced by using annotations on the enum values, this is for quicker implementation
        private static decimal GetMultiplierBasedOnCoverType(CoverType coverType)
        {
            return coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m // Default for other types
            };
        }
    }
}
