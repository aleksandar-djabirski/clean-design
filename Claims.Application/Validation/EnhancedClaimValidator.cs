using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using FluentValidation;

namespace Claims.Application.Validation
{
    public class EnhancedClaimValidator : AbstractValidator<Claim>
    {
        private readonly ICoverRepository _coverRepository; // Assuming you have such a repository/interface

        public EnhancedClaimValidator(ICoverRepository coverRepository)
        {
            _coverRepository = coverRepository;

            RuleFor(claim => claim).MustAsync(ValidClaimDateRange)
                .WithMessage("Claim's Created date must be within the period of the related Cover.");
        }

        private async Task<bool> ValidClaimDateRange(Claim claim, CancellationToken token)
        {
            var relatedCover = await _coverRepository.GetByIdAsync(claim.CoverId);
            if (relatedCover == null) return false;
            return DateOnly.FromDateTime(claim.Created.Date) >= relatedCover.StartDate && DateOnly.FromDateTime(claim.Created.Date) <= relatedCover.EndDate;
        }
    }
}
