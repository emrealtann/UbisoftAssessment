using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Data.Interfaces;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Data
{
    public class FeedbackContext : IFeedbackContext
    {
        public FeedbackContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Feedbacks = database.GetCollection<Feedback>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            FeedbackContextSeed.SeedData(Feedbacks);
        }

        public IMongoCollection<Feedback> Feedbacks { get; }
    }
}
