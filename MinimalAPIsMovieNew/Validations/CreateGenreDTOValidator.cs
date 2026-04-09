using FluentValidation;
using MinimalAPIsMovieNew.DTOs;
using MinimalAPIsMovieNew.Repositories;

namespace MinimalAPIsMovieNew.Validations
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeValueId = httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;


            if (routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id);
            }

            RuleFor(p => p.Name).NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150)
                    .WithMessage(ValidationUtilities.MaximumLengthMessage)
                    .Must(ValidationUtilities.FirstLetterIsUppercase)
                    .WithMessage(ValidationUtilities.FirstLetterIsUppareCaseMessage)
                    .MustAsync(async (name, _) =>
                    {
                        var exists = await genresRepository.Exists(id, name);
                        return !exists;
                    }).WithMessage(g => $"A genre with the name {g.Name} already exists");

        }


    }
}
