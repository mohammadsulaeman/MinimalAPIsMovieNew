using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovieNew.DTOs;
using MinimalAPIsMovieNew.Entities;
using MinimalAPIsMovieNew.Filters;
using MinimalAPIsMovieNew.Repositories;

namespace MinimalAPIsMovieNew.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));
            //.RequireAuthorization();

            group.MapGet("/{id:int}", GetGenresById);

            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");

            group.MapPut("/{idLint}", Update)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin")
                .WithOpenApi(options =>
                {
                    options.Summary = "Update a genre";
                    options.Description = "With this endpoint we can update a genre";
                    options.Parameters[0].Description = "The Id of the genre to Update";
                    options.RequestBody.Description = "The genre update";
                    return options;
                });

            group.MapDelete("/{id:int}", Delete)
                .RequireAuthorization("isadmin");
            return group;
        }


        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository genresRepository,
            IMapper mapper, ILoggerFactory loggerFactory)
        {
            var type = typeof(GenresEndpoints);
            var logger = loggerFactory.CreateLogger(type.FullName!);
            logger.LogInformation("Getting Get List All Genres");

            var genres = await genresRepository.GetAll();
            var genresDTOs = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTOs);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetGenresById(
         [AsParameters] GetGenreByIdRequestDTO model)
        {
            var genre = await model.Repository.GetById(model.Id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDTOs = model.Mapper.Map<GenreDTO>(genre);

            return TypedResults.Ok(genreDTOs);
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO,
            [AsParameters] CreateGenreRequestDTO model)
        {


            var genre = model.Mapper.Map<Genre>(createGenreDTO);
            await model.Repository.Create(genre);
            await model.OutputCacheStore.EvictByTagAsync("genres-get", default);
            var genreDTOs = model.Mapper.Map<GenreDTO>(genre);
            return TypedResults.Created($"/genres/{genre.Id}", genreDTOs);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, CreateGenreDTO createGenreDTO,
            IGenresRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {

            var exists = await repository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }
            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
