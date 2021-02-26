using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Data
{
    public class FeedbackContextSeed
    {
        public static void SeedData(IMongoCollection<Feedback> feedbackCollection)
        {
            bool existFeedback = feedbackCollection.Find(p => true).Any();
            if (!existFeedback)
            {
                feedbackCollection.InsertManyAsync(GetPreconfiguredFeedbacks());
            }
        }

        private static IEnumerable<Feedback> GetPreconfiguredFeedbacks()
        {
            return new List<Feedback>()
            {
                new Feedback()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    SessionId = "1",
                    UserId = "1",
                    Rating = 1,
                    Comment = "That was horrible!",
                    CreatedOn = DateTime.UtcNow.AddDays(-3)
                },
                new Feedback()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    SessionId = "1",
                    UserId = "2",
                    Rating = 2,
                    Comment = "Meh...",
                    CreatedOn = DateTime.UtcNow.AddDays(-2)
                },
                new Feedback()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    SessionId = "1",
                    UserId = "3",
                    Rating = 3,
                    Comment = "I just can't decide.",
                    CreatedOn = DateTime.UtcNow.AddDays(-1)
                },
                new Feedback()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    SessionId = "2",
                    UserId = "1",
                    Rating = 4,
                    Comment = "So far so good.",
                    CreatedOn = DateTime.UtcNow.AddDays(-3)
                },
                new Feedback()
                {
                    Id = "602d2149e773f2a3990b47f9",
                    SessionId = "3",
                    UserId = "1",
                    Rating = 5,
                    Comment = "That was an amazing experience!",
                    CreatedOn = DateTime.UtcNow.AddDays(-3)
                }
            };
        }
    }
}
