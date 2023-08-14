using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure
{
    public class CosmosDbService<T> where T : class
    {
        private readonly CosmosClient _client;
        private readonly Database _database;
        private readonly Container _container;

        public CosmosDbService(CosmosClient client, string databaseName)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _database = _client.GetDatabase(databaseName);
            var containerName = typeof(T).Name; // assuming container names are same as type names.
            _container = _database.GetContainer(containerName);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            List<T> results = new List<T>();
            FeedIterator<T> iterator = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));

            while (iterator.HasMoreResults)
            {
                FeedResponse<T> response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<T> GetByIdAsync<T>(string id) where T : class
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddAsync<T>(T item) where T : class
        {
            await _container.CreateItemAsync(item);
        }

        public async Task UpdateAsync<T>(string id, T item) where T : class
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<object>(id, new PartitionKey(id));
        }
    }
}
