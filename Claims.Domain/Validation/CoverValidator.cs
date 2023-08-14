using Claims.Domain.Entities;
using FluentValidation;

namespace Claims.Domain.Validation
{
    public class CoverValidator : AbstractValidator<Cover>
    {
        public CoverValidator()
        {
            RuleFor(cover => cover.StartDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.Date));
            RuleFor(cover => (cover.EndDate.DayOfYear - cover.StartDate.DayOfYear))
                .LessThanOrEqualTo(365)
                .WithMessage("Total insurance period cannot exceed 1 year");
        }
    }
}
