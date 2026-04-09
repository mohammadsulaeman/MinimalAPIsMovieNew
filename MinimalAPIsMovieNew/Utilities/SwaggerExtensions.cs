using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MinimalAPIsMovieNew.DTOs;

namespace MinimalAPIsMovieNew.Utilities
{
    public static class SwaggerExtensions
    {
        public static TBuilder AddMoviesFilterParameters<TBuilder>(this TBuilder builder)
          where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(options =>
            {
                AddPaginationParameters(options);

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "Title",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "string",
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "GenreId",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "Integer",
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "IsTheaters",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "boolean",
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "FutureReleases",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "boolean",
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "OrderByField",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "string",
                        Enum = new List<IOpenApiAny>()
                        {
                            new OpenApiString("Title"),
                            new OpenApiString("ReleaseDate")
                        }
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "OrderByAscending",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                    {
                        Type = "boolean",
                    }
                });

                return options;
            });
        }

        public static TBuilder AddPaginationParameters<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(options =>
            {
                AddPaginationParameters(options);

                return options;
            });
        }

        private static void AddPaginationParameters(OpenApiOperation openApiOperation)
        {
            openApiOperation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
            {
                Name = "Page",
                In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "Integer",
                    Default = new OpenApiInteger(PaginationDTO.PageInitialValue)
                }
            });

            openApiOperation.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
            {
                Name = "RecordsPerPage",
                In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "Integer",
                    Default = new OpenApiInteger(PaginationDTO.RecordsPerPageValue)
                }
            });
        }
    }
}
