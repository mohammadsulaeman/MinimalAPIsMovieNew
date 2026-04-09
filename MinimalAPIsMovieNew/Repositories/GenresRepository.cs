using Microsoft.Data.SqlClient;
using MinimalAPIsMovieNew.Entities;
using System.Data;
using Dapper;

namespace MinimalAPIsMovieNew.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string connectionString;
        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var id = await connection.QuerySingleAsync<int>("Genres_Create", new { genre.Name },
                    commandType: System.Data.CommandType.StoredProcedure);
                genre.Id = id;
                return id;
            }


        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Genres_DeleteByID", new { id },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>
                    (@"Genres_Exists", new { id },
                    commandType: System.Data.CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll",
                    commandType: System.Data.CommandType.StoredProcedure);
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>
                    (@"Genres_GetByID", new { id }, commandType: System.Data.CommandType.StoredProcedure);
                return genre;
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Update", new { genre.Id, genre.Name },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<List<int>> Exists(List<int> ids)
        {

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genreId in ids)
            {
                dt.Rows.Add(genreId);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var idsOfGenresThatExists = await connection
                    .QueryAsync<int>("Genres_GetBySeveralIds", new
                    {
                        genresIds = dt
                    }, commandType: CommandType.StoredProcedure);
                return idsOfGenresThatExists.ToList();
            }
        }

        public async Task<bool> Exists(int id, string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>("Genres_ExistsByIdAndName",
                    new { id, name }, commandType: CommandType.StoredProcedure);

                return exists;
            }
        }
    }
}
