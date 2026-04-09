using MinimalAPIsMovieNew.Entities;

namespace MinimalAPIsMovieNew.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
        Task<List<int>> Exists(List<int> ids);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
        Task Update(Genre genre);
    }
}