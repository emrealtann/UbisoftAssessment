using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Data.Interfaces;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Services.Interfaces;

namespace UbisoftAssessment.Services
{
    /// <summary>
    /// Repository class for the feedback collection. Can be used for getting feedbacks or CRUD operations.
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackContext _context;

        /// <summary>
        /// Constructor method for the repository class.
        /// </summary>
        /// <param name="context">Feedback context.</param>
        public FeedbackService(IFeedbackContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Creates a feedback in the MongoDB feedback collection.
        /// </summary>
        /// <param name="feedback">Feedback entity to create.</param>
        public async Task CreateFeedback(Feedback feedback)
        {
            await _context.Feedbacks.InsertOneAsync(feedback);
        }

        /// <summary>
        /// Lists the last 15 feedbacks left by players and allows filtering by rating.
        /// </summary>
        /// <param name="rating">Rating filter. Can be between 1 and 5.</param>
        /// <returns>Feedback list.</returns>
        public async Task<IEnumerable<Feedback>> GetFeedbacks(int? rating = null)
        {
            FilterDefinition<Feedback> filter = Builders<Feedback>.Filter.Empty;

            if (rating.HasValue)
            {
                if (rating < 1 || rating > 5)
                {
                    throw new Exception(FeedbackVerificationResult.RatingInappropriate.ToString());
                }
                else
                {
                    filter = Builders<Feedback>.Filter.Eq(f => f.Rating, rating);
                }
            }

            return await _context
                            .Feedbacks
                            .Find(filter)
                            .SortByDescending(f => f.CreatedOn)
                            .Limit(15)
                            .ToListAsync();
        }

        /// <summary>
        /// Verifies the feedback to be inserted.
        /// </summary>
        /// <param name="feedback">Feedback entity to be inserted.</param>
        /// <returns>
        /// Returns FeedbackVerificationResult enum. Returns Ok if everything is ok.
        /// If it doesn't return ok, then there is a problem with the entity values.
        /// You can get the error message with the returned enum from the localization resource.
        /// </returns>
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
