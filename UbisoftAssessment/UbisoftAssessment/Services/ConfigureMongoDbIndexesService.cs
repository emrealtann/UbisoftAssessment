
using Microsoft.Extensions.Configuration;
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
    /// <summary>
    /// Configuration service for creating MongoDB indexes during the application start.
    /// </summary>
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly ILogger<ConfigureMongoDbIndexesService> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor method of ConfigureMongoDbIndexesService class.
        /// </summary>
        /// <param name="configuration">Configuration object to be used for accessing database settings.</param>
        /// <param name="logger">Logger object to be used for logging purposes.</param>
        public ConfigureMongoDbIndexesService(IConfiguration configuration, ILogger<ConfigureMongoDbIndexesService> logger)
            => (_configuration, _logger) = (configuration, logger);

        /// <summary>
        /// IHostedService override method. Creates MongoDB indexes for Rating and CreatedOn fields.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new MongoClient(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(_configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            var collection = database.GetCollection<Feedback>(_configuration.GetValue<string>("DatabaseSettings:CollectionName"));

            _logger.LogInformation("Creating indexes on feedbacks");

            //creates an index for the Rating field for faster search and filtering.
            var indexKeysDefinition = Builders<Feedback>.IndexKeys.Ascending(x => x.Rating);

            //creates an index for the CreatedOn field with descending order for faster sorting.
            var indexKeysDefinition2 = Builders<Feedback>.IndexKeys.Descending(x => x.CreatedOn);

            //adds the indexes to the list
            List<CreateIndexModel<Feedback>> createIndexModels = new List<CreateIndexModel<Feedback>>();
            createIndexModels.Add(new CreateIndexModel<Feedback>(indexKeysDefinition));
            createIndexModels.Add(new CreateIndexModel<Feedback>(indexKeysDefinition2));

            //creates the indexes on mongodb collection
            await collection.Indexes.CreateManyAsync(createIndexModels, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// IHostedService override method. Stops the task execution.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
