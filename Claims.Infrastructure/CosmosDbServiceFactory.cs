using Claims.Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure
{
    public class CosmosDbServiceFactory : ICosmosDbServiceFactory
    {
        private readonly CosmosClient _client;
        private readonly string _databaseName;

        public CosmosDbServiceFactory(CosmosClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
        }

        public CosmosDbService<T> Create<T>() where T : class
        {
            return new CosmosDbService<T>(_client, _databaseName);
        }
    }
}
