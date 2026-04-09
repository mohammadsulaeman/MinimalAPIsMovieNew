using FluentValidation;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("The field {PropertyName} is required")
                .MaximumLength(150)
                 .WithMessage("The field {PropertyName} should be less than {MaxLength} characters");

            var minimumDate = new DateTime(1990, 1, 1);

            RuleFor(p => p.DateOfBirth).GreaterThanOrEqualTo(minimumDate)
                .WithMessage(ValidationUtilities.GreateThanDate(minimumDate));

        }
    }
}
