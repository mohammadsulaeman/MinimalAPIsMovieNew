using MinimalAPIsMovieNew.DTOs;
using MinimalAPIsMovieNew.Entities;

namespace MinimalAPIsMovieNew.Repositories
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<ActorMovie> actors);
        Task Assign(int id, List<int> genresIds);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Movie>> Filter(MoviesFilterDTO moviesFilterDTO);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
    }
}