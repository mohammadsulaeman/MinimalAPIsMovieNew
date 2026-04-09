using FluentValidation;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Validations
{
    public class UserCredentialDTOValidator : AbstractValidator<UserCredentialDTO>
    {
        public UserCredentialDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(256)
                .WithMessage(ValidationUtilities.MaximumLengthMessage)
                .EmailAddress().WithMessage(ValidationUtilities.EmailAddressMessage);

            RuleFor(x => x.Password).NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage);
        }
    }
}
