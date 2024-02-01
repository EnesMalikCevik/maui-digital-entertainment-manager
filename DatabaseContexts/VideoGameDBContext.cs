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
    public class VideoGameDBContext
    {
        public const string DatabaseFilename = "Test2.db3";

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

        public VideoGameDBContext() { }

        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database =>

            (_connection ??= new SQLiteAsyncConnection(DatabasePath, Flags));

        private readonly string cosmosConnectionString = "null";
        private readonly string cosmosDatabaseId = "MediaDatabase";
        private readonly string cosmosContainerId = "VideoGamesContainer";

        private CosmosClient _cosmosClient;
        private CosmosClient CosmosClient => _cosmosClient ??= new CosmosClient(cosmosConnectionString);
        
        async Task Init()
        {
            await Database.CreateTableAsync<VideoGame>();
            InitializeCosmosDB();
        }


        private async Task InitializeCosmosDB()
        {
            var cosmosClient = CosmosClient;
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDatabaseId);
            var database = databaseResponse.Database;

            var containerResponse = await database.CreateContainerIfNotExistsAsync(cosmosContainerId, "/UserId");
        }

        public async Task<List<VideoGame>> GetVideoGamesAsync(string userId)
        {
            await Init();
            var sqliteVideoGames = await Database.Table<VideoGame>().Where(m => m.UserId == userId).ToListAsync();
            var cosmosVideoGames = await GetCosmosVideoGamesAsync(userId);
            return cosmosVideoGames;
            // Merge SQLite and Cosmos DB results
            /*var commonVideoGames = sqliteVideoGames.Join(cosmosVideoGames,
                                       sqliteVideoGames => sqliteVideoGames.CosmosId,
                                       cosmosVideoGames => cosmosVideoGames.CosmosId,
                                       (sqliteVideoGames, cosmosVideoGames) => sqliteVideoGames)
                                   .ToList();
            var uniqueSqliteVideoGames = sqliteVideoGames.Except(commonVideoGames).ToList();

            var mergedVideoGames = uniqueSqliteVideoGames.Concat(cosmosVideoGames).ToList();

            return mergedVideoGames;*/
        }


        private async Task<List<VideoGame>> GetCosmosVideoGamesAsync(string userId)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
            var iterator = container.GetItemQueryIterator<VideoGame>(
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(userId) }
            );

            var cosmosVideoGames = new List<VideoGame>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                cosmosVideoGames.AddRange(response.Resource);
            }

            return cosmosVideoGames;
        }



        /*public async Task<int> SaveGameAsync(VideoGame videogame)
        {
            await Init();
            if (videogame.ID != 0)
                return await Database.UpdateAsync(videogame);
            else
                return await Database.InsertAsync(videogame);
        }

        public async Task SaveAllGamesAsync(IEnumerable<VideoGame> videogames)
        {
            await Init();
            foreach (var videogame in videogames)
            {
                if (videogame.ID != 0)
                    await Database.UpdateAsync(videogame);
                else
                    await Database.InsertAsync(videogame);
            }
        }*/

        public async Task<int> SaveVideoGameAsync(VideoGame videogame, string userId)
        {
            await Init();
            videogame.UserId = userId;
            if (videogame.ID != 0)
                /*return*/
                await Database.UpdateAsync(videogame);
            else
                /*return*/
                await Database.InsertAsync(videogame);

            await SaveToCosmosDB(videogame);

            return videogame.ID;
        }


        private async Task SaveToCosmosDB(VideoGame videogame)
        {
            var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);

            // Ensure the CosmosId is set
            if (string.IsNullOrEmpty(videogame.CosmosId))
            {
                videogame.CosmosId = Guid.NewGuid().ToString();
            }

            await container.UpsertItemAsync(videogame, new PartitionKey(videogame.UserId));
        }


        public async Task SaveAllVideoGamesAsync(IEnumerable<VideoGame> videogames, string userId)
        {
            await Init();
            foreach (var tvshow in videogames)
            {
                await SaveVideoGameAsync(tvshow, userId);
                /*movie.UserId = userId;
                if (movie.ID != 0)
                    await Database.UpdateAsync(movie);
                else
                    await Database.InsertAsync(movie);*/

            }
        }



        /* public async Task<int> DeleteGameAsync(VideoGame videogame)
         {
             await Init();
             return await Database.DeleteAsync(videogame);
         }*/
        public async Task<int> DeleteVideoGameAsync(VideoGame videogame)
        {
            await Init();
            /*return*/
            var deletedRows = await Database.DeleteAsync(videogame);

            if (!string.IsNullOrEmpty(videogame.CosmosId))
            {
                var container = CosmosClient.GetContainer(cosmosDatabaseId, cosmosContainerId);
                await container.DeleteItemAsync<VideoGame>(videogame.CosmosId, new PartitionKey(videogame.UserId));
            }

            return deletedRows;
        }


    }
}