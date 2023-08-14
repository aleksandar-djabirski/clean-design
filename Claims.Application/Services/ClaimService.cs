using AutoMapper;
using Claims.Application.DataTransferObjects;
using Claims.Application.Interfaces.Repositories;
using Claims.Application.Interfaces.Services;
using FluentValidation;
using Claim = Claims.Domain.Entities.Claim;

namespace Claims.Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IClaimAuditRepository _claimAuditRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<Claim> _claimValidator;

        public ClaimService(IClaimRepository claimRepository, IClaimAuditRepository claimAuditRepository, IMapper mapper, IValidator<Claim> claimValidator)
        {
            _claimRepository = claimRepository ?? throw new ArgumentNullException(nameof(claimRepository));
            _claimAuditRepository = claimAuditRepository ?? throw new ArgumentNullException(nameof(claimAuditRepository));
            _mapper = mapper;
            _claimValidator = claimValidator;
        }

        public async Task<IEnumerable<ClaimDataTransferObject>> GetAllAsync()
        {
            var claims = await _claimRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClaimDataTransferObject>>(claims);
        }

        public async Task<ClaimDataTransferObject> GetByIdAsync(string id)
        {
            var claim = await _claimRepository.GetByIdAsync(id);
            return _mapper.Map<ClaimDataTransferObject>(claim);
        }

        public async Task<ClaimDataTransferObject> CreateAsync(ClaimDataTransferObject claimDto)
        {
            var claim = _mapper.Map<Claim>(claimDto);
            claim.Id = Guid.NewGuid().ToString();

            var validationResult = await _claimValidator.ValidateAsync(claim);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _claimRepository.AddAsync(claim);
            await _claimAuditRepository.AuditAsync(claim.Id, "POST");
            return _mapper.Map<ClaimDataTransferObject>(claim);
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _claimRepository.DeleteAsync(id);
            await _claimAuditRepository.AuditAsync(id, "DELETE");
        }
    }
}
