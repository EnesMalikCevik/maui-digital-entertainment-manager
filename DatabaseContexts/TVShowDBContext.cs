using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp4.Models;
using Microsoft.Azure.Cosmos;
using SQLite;

namespace MauiApp4.DatabaseContexts
{
    public class TVShowDBContext
    {
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

        public TVShowDBContext() { }

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database =>
        
            (_connection ??= new SQLiteAsyncConnection(DatabasePath, Flags));

        private readonly string cosmosConnectionString = "null";
        private readonly string cosmosDatabaseId = "MediaDatabase";
        private readonly string cosmosContainerId = "TVShowsContainer";

        private CosmosClient _cosmosClient;
        private CosmosClient CosmosClient => _cosmosClient ??= new CosmosClient(cosmosConnectionString);

        async Task Init()
        {
            await Database.CreateTableAsync<TVShow>();
            await InitializeCosmosDB();
        }




        private async Task InitializeCosmosDB()
        {
            var cosmosClient = CosmosClient;
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDatabaseId);
            var database = databaseResponse.Database;
            var containerResponse = await database.CreateContainerIfNotExistsAsync(cosmosContainerId, "/UserId");
        }


        public async Task<List<TVShow>> GetTVShowsAsync(string userId)
        {
            await Init();
            var sqliteTVShows = await Database.Table<TVShow>().Where(m => m.UserId == userId).ToListAsync();
            var cosmosTVShows = await GetCosmosTVShowsAsync(userId);
            return cosmosTVShows;
        }


        private async Task<List<TVShow>> GetCosmosTVShowsAsync(string userId)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
            var iterator = container.GetItemQueryIterator<TVShow>(
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(userId) }
            );

            var cosmosTVShows = new List<TVShow>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                cosmosTVShows.AddRange(response.Resource);
            }

            return cosmosTVShows;
        }



        public async Task<int> SaveTVShowAsync(TVShow tvshow, string userId)
        {
            await Init();
            tvshow.UserId = userId;
            if (tvshow.ID != 0)
                await Database.UpdateAsync(tvshow);
            else
                await Database.InsertAsync(tvshow);

            await SaveToCosmosDB(tvshow);

            return tvshow.ID;
        }


        private async Task SaveToCosmosDB(TVShow tvshow)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);

            if (string.IsNullOrEmpty(tvshow.CosmosId))
            {
                tvshow.CosmosId = Guid.NewGuid().ToString();
            }

            await container.UpsertItemAsync(tvshow, new PartitionKey(tvshow.UserId));
        }


        public async Task SaveAllTVShowsAsync(IEnumerable<TVShow> tvshows, string userId)
        {
            await Init();
            foreach (var tvshow in tvshows)
            {
                await SaveTVShowAsync(tvshow, userId);
            }
        }

        public async Task<int> DeleteTVShowAsync(TVShow tvshow)
        {
            await Init();
            var deletedRows = await Database.DeleteAsync(tvshow);

            if (!string.IsNullOrEmpty(tvshow.CosmosId))
            {
                var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
                await container.DeleteItemAsync<TVShow>(tvshow.CosmosId, new PartitionKey(tvshow.UserId));
            }

            return deletedRows;
        }


    }
}