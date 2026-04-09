using FluentValidation;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Validations
{
    public class CreateCommentDTOValidation : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidation()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
        }
    }
}
