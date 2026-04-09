using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMovieNew.Services
{
    public interface IUserService
    {
        Task<IdentityUser?> GetUser();
    }
}