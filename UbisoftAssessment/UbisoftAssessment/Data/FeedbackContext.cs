﻿using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Data.Interfaces;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Data
{
    /// <summary>
    /// MongoDB context for the feedbacks.
    /// </summary>
    public class FeedbackContext : IFeedbackContext
    {
        /// <summary>
        /// Constructor method for FeedbackContext.
        /// </summary>
        /// <param name="configuration">Configuration object to be used for accessing database settings.</param>
        public FeedbackContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Feedbacks = database.GetCollection<Feedback>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            FeedbackContextSeed.SeedData(Feedbacks);
        }

        /// <summary>
        /// Feedback collection object 
        /// </summary>
        public IMongoCollection<Feedback> Feedbacks { get; }
    }
}
