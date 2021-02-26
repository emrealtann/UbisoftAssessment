using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Data.Interfaces;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Repositories.Interfaces;

namespace UbisoftAssessment.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IFeedbackContext _context;

        public FeedbackRepository(IFeedbackContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateFeedback(Feedback feedback)
        {
            await _context.Feedbacks.InsertOneAsync(feedback);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacks(int? rating = null)
        {
            FilterDefinition<Feedback> filter = Builders<Feedback>.Filter.Empty;

            if (rating.HasValue)
            {
                filter = Builders<Feedback>.Filter.Eq(f => f.Rating, rating);
            }

            return await _context
                            .Feedbacks
                            .Find(filter)
                            .SortByDescending(f => f.CreatedOn)
                            .Limit(15)
                            .ToListAsync();
        }

        public async Task<FeedbackVerificationResult> VerifyFeedback(Feedback feedback)
        {
            if (string.IsNullOrEmpty(feedback.UserId))
            {
                return FeedbackVerificationResult.UserIdEmpty;
            }
            
            if(string.IsNullOrEmpty(feedback.SessionId))
            {
                return FeedbackVerificationResult.SessionIdEmpty;
            }
            
            if(feedback.Rating < 1 || feedback.Rating > 5)
            {
                return FeedbackVerificationResult.RatingInappropriate;
            }


            FilterDefinition<Feedback> filter1 = Builders<Feedback>.Filter.Eq(f => f.SessionId, feedback.SessionId);
            FilterDefinition<Feedback> filter2 = Builders<Feedback>.Filter.Eq(f => f.UserId, feedback.UserId);
            FilterDefinition<Feedback> filter = Builders<Feedback>.Filter.And(filter1, filter2);

            bool hasFeedback = await _context.Feedbacks.Find(filter).AnyAsync();

            if(hasFeedback)
            {
                return FeedbackVerificationResult.UserAlreadyHasFeedback;
            }

            return FeedbackVerificationResult.Ok;
        }
    }
}
