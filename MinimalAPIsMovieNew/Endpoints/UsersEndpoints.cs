using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMovieNew.DTOs;
using MinimalAPIsMovieNew.Filters;
using MinimalAPIsMovieNew.Services;
using MinimalAPIsMovieNew.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPIsMovieNew.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group)
        {
            group.MapPost("/register", Register)
                .AddEndpointFilter<ValidationFilter<UserCredentialDTO>>();
            group.MapPost("/login", Login)
                .AddEndpointFilter<ValidationFilter<UserCredentialDTO>>();

            group.MapPost("/makeadmin", MakeAdmin)
                .AddEndpointFilter<ValidationFilter<EditClaimDTO>>()
                .RequireAuthorization();

            group.MapPost("/removeadmin", RemoveAdmin)
                .AddEndpointFilter<ValidationFilter<EditClaimDTO>>()
                .RequireAuthorization("isadmin");

            group.MapGet("/renewtoken", Renew)
                .RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<AuthenticationResponseDTO>,
            BadRequest<IEnumerable<IdentityError>>>> Register(UserCredentialDTO userCredentialDTO,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialDTO.Email,
                Email = userCredentialDTO.Email,
            };

            var result = await userManager.CreateAsync(user, userCredentialDTO.Password);

            if (result.Succeeded)
            {

                var authenticationResponse = await BuildToken(userCredentialDTO, configuration, userManager);
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest(result.Errors);
            }

        }

        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<string>>> Login(
            UserCredentialDTO userCredentialDTO,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            var user = await userManager.FindByEmailAsync(userCredentialDTO.Email);

            if (user is null)
            {
                return TypedResults.BadRequest("There was a problem with the email or the password");
            }

            var results = await signInManager.CheckPasswordSignInAsync(user,
                userCredentialDTO.Password, lockoutOnFailure: false);

            if (results.Succeeded)
            {
                var authenticationResponse = await BuildToken(userCredentialDTO, configuration, userManager);
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest("There was a problem with the email or the password");
            }
        }

        private async static Task<AuthenticationResponseDTO> BuildToken(UserCredentialDTO userCredentialDTO,
            IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email",userCredentialDTO.Email),
                new Claim("Whatever I want","this is a value")
            };

            var user = await userManager.FindByNameAsync(userCredentialDTO.Email);
            var claimsFromDB = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimsFromDB);

            var key = KeyHandler.GetKey(configuration).First();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiration, signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);


            return new AuthenticationResponseDTO
            {
                Token = token,
                Expiration = expiration
            };
        }

        static async Task<Results<NoContent, NotFound>> MakeAdmin(EditClaimDTO editClaimDTO,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            await userManager.AddClaimAsync(user, new Claim("isadmin", "true"));
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> RemoveAdmin(EditClaimDTO editClaimDTO,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            await userManager.RemoveClaimAsync(user, new Claim("isadmin", "true"));
            return TypedResults.NoContent();
        }

        private static async Task<Results<NotFound, Ok<AuthenticationResponseDTO>>> Renew(
            IUserService userService, IConfiguration configuration,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var user = await userService.GetUser();

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            var usersCredential = new UserCredentialDTO { Email = user.Email! };
            var response = await BuildToken(usersCredential, configuration, userManager);
            return TypedResults.Ok(response);
        }
    }
}
