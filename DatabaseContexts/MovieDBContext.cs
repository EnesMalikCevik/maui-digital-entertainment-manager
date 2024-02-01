using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp4.Models;

using SQLite;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace MauiApp4.DatabaseContexts
{
    public class MovieDBContext
    {
        public MovieDBContext() { }

        public const string DatabaseFilename = "Test3.db3";
        public static string DatabasePath
        {
            get
            {
                return Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

            }
        }
        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database =>

            (_connection ??= new SQLiteAsyncConnection(DatabasePath, Flags));

        private readonly string cosmosConnectionString = "null";
        private readonly string cosmosDatabaseId = "MediaDatabase";
        private readonly string cosmosContainerId = "MoviesContainer";

        private CosmosClient _cosmosClient;
        private CosmosClient CosmosClient => _cosmosClient ??= new CosmosClient(cosmosConnectionString);


        async Task Init()
        {
            await Database.CreateTableAsync<Movie>();
            await InitializeCosmosDB();
        }


        private async Task InitializeCosmosDB()
        {
            var cosmosClient = CosmosClient;
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDatabaseId);
            var database = databaseResponse.Database;
            var containerResponse = await database.CreateContainerIfNotExistsAsync(cosmosContainerId, "/UserId");
        }


        public async Task<List<Movie>> GetMoviesAsync(string userId)
        {
            await Init();
            var sqliteMovies = await Database.Table<Movie>().Where(m => m.UserId == userId).ToListAsync();
            var cosmosMovies = await GetCosmosMoviesAsync(userId);
            return cosmosMovies;
        
        }

        private async Task<List<Movie>> GetCosmosMoviesAsync(string userId)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
            var iterator = container.GetItemQueryIterator<Movie>(
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(userId) }
            );

            var cosmosMovies = new List<Movie>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                cosmosMovies.AddRange(response.Resource);
            }

            return cosmosMovies;
        }





        public async Task<int> SaveMovieAsync(Movie movie)
        {
            await Init();
            
            if (movie.ID != 0)
                await Database.UpdateAsync(movie);
            else
                await Database.InsertAsync(movie);

            await SaveToCosmosDB(movie);

            return movie.ID;
        }


        private async Task SaveToCosmosDB(Movie movie)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);

            if (string.IsNullOrEmpty(movie.CosmosId))
            {
                movie.CosmosId = Guid.NewGuid().ToString();
            }

            await container.UpsertItemAsync(movie, new PartitionKey(movie.UserId));
        }

        public async Task<int> DeleteMovieAsync(Movie movie)
        {
            await Init();
            var deletedRows = await Database.DeleteAsync(movie);

            if (!string.IsNullOrEmpty(movie.CosmosId))
            {
                var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
                await container.DeleteItemAsync<Movie>(movie.CosmosId, new PartitionKey(movie.UserId));
            }

            return deletedRows;
        }


    }
}