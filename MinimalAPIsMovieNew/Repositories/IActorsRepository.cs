using MinimalAPIsMovieNew.DTOs;
using MinimalAPIsMovieNew.Entities;

namespace MinimalAPIsMovieNew.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<int>> Exists(List<int> ids);
        Task<List<Actor>> GetAllActors(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name);
        Task Update(Actor actor);
    }
}