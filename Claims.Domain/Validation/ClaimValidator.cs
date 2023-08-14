using Claims.Domain.Entities;
using FluentValidation;

namespace Claims.Domain.Validation
{
    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(claim => claim.DamageCost).LessThanOrEqualTo(100000);
        }
    }
}
