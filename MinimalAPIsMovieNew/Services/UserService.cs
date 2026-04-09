using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMovieNew.Services
{
    public class UserService(IHttpContextAccessor httpContextAccessor,
        UserManager<IdentityUser> userManager) : IUserService
    {

        public async Task<IdentityUser?> GetUser()
        {
            var emailClaim = httpContextAccessor.HttpContext!
                .User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (emailClaim is null)
            {
                return null;
            }

            var email = emailClaim.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
