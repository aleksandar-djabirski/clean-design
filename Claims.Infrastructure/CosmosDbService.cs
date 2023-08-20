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

        public async Task<IEnumerable<T>> GetAllAsync()
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

        public async Task<T> GetByIdAsync(string id)
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

        public async Task AddAsync(T item)
        {
            await _container.CreateItemAsync(item);
        }

        public async Task UpdateAsync(string id, T item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }
    }
}
