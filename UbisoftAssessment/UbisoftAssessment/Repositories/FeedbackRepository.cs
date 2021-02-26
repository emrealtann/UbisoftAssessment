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

        public async Task<IEnumerable<Feedback>> GetFeedbacks(int rating)
        {
            FilterDefinition<Feedback> filter = Builders<Feedback>.Filter.Eq(f => f.Rating, rating);

            return await _context
                            .Feedbacks
                            .Find(filter)
                            .SortByDescending(f => f.CreatedOn)
                            .Limit(15)
                            .ToListAsync();
        }
    }
}
