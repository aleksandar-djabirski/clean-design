using Claims.Application.Interfaces.Repositories;
using Claims.Domain.Entities;
using Claims.Infrastructure.Configuration;
using Claims.Infrastructure.Data.DbContexts;
using Claims.Infrastructure.Interfaces;
using Claims.Infrastructure.Repositories;
using Claims.Infrastructure.Repositories.Claims.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Claims.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // CosmosDB setup
            var cosmosSettings = configuration.GetSection("CosmosDb").Get<CosmosDbSettings>();

            services.AddSingleton(cosmosSettings);

            CosmosClientOptions clientOptions = new CosmosClientOptions
            {
                Serializer = new CosmosCamelCaseJsonSerializer()
            };

            var client = new CosmosClient(cosmosSettings.Account, cosmosSettings.Key, clientOptions);
            services.AddSingleton(client);

            InitializeCosmosClientInstanceAsync(cosmosSettings).GetAwaiter().GetResult();

            services.AddSingleton<CosmosDbService<Claim>>(provider =>
                new CosmosDbService<Claim>(client, cosmosSettings.DatabaseName));
            services.AddSingleton<CosmosDbService<Cover>>(provider =>
                new CosmosDbService<Cover>(client, cosmosSettings.DatabaseName));

            // EF setup
            services.AddDbContext<AuditDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IClaimRepository, ClaimRepository>();
            services.AddTransient<ICoverRepository, CoverRepository>();
            services.AddTransient<IClaimAuditRepository, ClaimAuditRepository>();
            services.AddTransient<ICoverAuditRepository, CoverAuditRepository>();

            var azureStorageSettings = configuration.GetSection("AzureStorage").Get<StorageSettings>();

            services.AddSingleton<IQueueService>(new QueueService(azureStorageSettings.ConnectionString, azureStorageSettings.QueueName));

            return services;
        }

        static async Task InitializeCosmosClientInstanceAsync(CosmosDbSettings settings)
        {
            CosmosClient client = new CosmosClient(settings.Account, settings.Key);

            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName);
            foreach (var containerSetting in settings.Containers)
            {
                string containerName = containerSetting.Key;
                string partitionKeyPath = containerSetting.Value;
                await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath);
            }
        }
    }
}
