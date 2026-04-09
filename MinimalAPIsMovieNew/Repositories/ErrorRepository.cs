using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using MinimalAPIsMovieNew.Entities;

namespace MinimalAPIsMovieNew.Repositories
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly string? connectionString;

        public ErrorRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Guid> Create(Error error)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                error.Id = Guid.NewGuid();
                await connection.ExecuteAsync("Error_Create",
                    new
                    {
                        error.Id,
                        error.ErrorMessage,
                        error.StackTrace,
                        error.Date
                    }, commandType: CommandType.StoredProcedure);

                return error.Id;

            }
        }
    }
}
