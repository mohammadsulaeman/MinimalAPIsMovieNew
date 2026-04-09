using FluentValidation;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Validations
{
    public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.MaximumLengthMessage);
        }
    }
}
