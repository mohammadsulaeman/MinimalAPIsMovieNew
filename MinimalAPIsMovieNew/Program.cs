using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIsMovieNew.Endpoints;
using MinimalAPIsMovieNew.Entities;
using MinimalAPIsMovieNew.Repositories;
using MinimalAPIsMovieNew.Services;
using MinimalAPIsMovieNew.Swagger;
using MinimalAPIsMovieNew.Utilities;

var builder = WebApplication.CreateBuilder(args);


// Security
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration.GetValue<string>("allowOriginURL")!)
            .AllowAnyMethod()
            .AllowAnyHeader().AllowAnyOrigin();
    });

    options.AddPolicy("free", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = KeyHandler.GetAllKeys(builder.Configuration)
        //IssuerSigningKey = KeyHandler.GetKey(builder.Configuration).First()
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
});

// REDIS
builder.Services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis");
});

// Repository Dan Services
builder.Services.AddTransient<IUserStore<IdentityUser>, UserStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorRepository, ErrorRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfiles>();
});

// Validations
builder.Services.AddHttpContextAccessor();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie API",
        Description = "This is a web api for working with movie data",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "mohammadsulaeman24@gmail.com",
            Name = "Mohammad Sulaeman",
            Url = new Uri("https://mohammadsulaeman.github.io/")
        }
    });


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Input JWT token: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[] {}
        }
    });

    options.OperationFilter<AuthorizationFilter>();
});

var app = builder.Build();
if (builder.Environment.IsDevelopment() 
    || builder.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\ImagesMovieNet"),
    RequestPath = ""
});

app.MapGet("/", () => "Hello World!");
// Penggunaan Cors
app.UseCors();

app.UseStaticFiles();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace; ;

    var repository = context.RequestServices.GetRequiredService<IErrorRepository>();
    await repository.Create(error);

    await Results
    .BadRequest(new
    {
        type = "error",
        message = "an unexpected exception has occured",
        status = 500
    })
    .ExecuteAsync(context);
}));
app.UseStatusCodePages();

// penggunaan cache
app.UseOutputCache();

//app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/error", () =>
{
    throw new InvalidOperationException("example error");
});

// Maps Genres
app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();
app.MapGroup("/users").MapUsers();

app.Run();
