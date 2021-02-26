
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Services
{
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoClient _client;
        private readonly ILogger<ConfigureMongoDbIndexesService> _logger;

        public ConfigureMongoDbIndexesService(IMongoClient client, ILogger<ConfigureMongoDbIndexesService> logger)
            => (_client, _logger) = (client, logger);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var database = _client.GetDatabase("example");
            var collection = database.GetCollection<Feedback>("Feedbacks");

            _logger.LogInformation("Creating indexes on feedbacks");
            var indexKeysDefinition = Builders<Feedback>.IndexKeys.Ascending(x => x.Rating);
            var indexKeysDefinition2 = Builders<Feedback>.IndexKeys.Descending(x => x.CreatedOn);

            List<CreateIndexModel<Feedback>> createIndexModels = new List<CreateIndexModel<Feedback>>();
            createIndexModels.Add(new CreateIndexModel<Feedback>(indexKeysDefinition));
            createIndexModels.Add(new CreateIndexModel<Feedback>(indexKeysDefinition2));

            //create index on mongodb collection for Rating and CreatedOn fields
            await collection.Indexes.CreateManyAsync(createIndexModels, cancellationToken: cancellationToken);
        }


        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
