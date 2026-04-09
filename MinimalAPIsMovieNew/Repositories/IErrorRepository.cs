using MinimalAPIsMovieNew.Entities;

namespace MinimalAPIsMovieNew.Repositories
{
    public interface IErrorRepository
    {
        Task<Guid> Create(Error error);
    }
}