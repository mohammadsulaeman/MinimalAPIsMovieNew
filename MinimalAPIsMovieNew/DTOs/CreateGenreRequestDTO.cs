using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovieNew.Repositories;

namespace MinimalAPIsMovieNew.DTOs
{
    public class CreateGenreRequestDTO
    {
        public IOutputCacheStore OutputCacheStore { get; set; } = null!;
        public IGenresRepository Repository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
