using FluentValidation;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Validations
{
    public class EditClaimDTOValidator : AbstractValidator<EditClaimDTO>
    {
        public EditClaimDTOValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage(ValidationUtilities.EmailAddressMessage);
        }
    }
}
