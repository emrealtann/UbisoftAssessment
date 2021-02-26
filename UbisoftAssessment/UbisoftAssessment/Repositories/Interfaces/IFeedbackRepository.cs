using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task CreateFeedback(Feedback feedback);
        Task<IEnumerable<Feedback>> GetFeedbacks(int? rating = null);
        Task<FeedbackVerificationResult> VerifyFeedback(Feedback feedback);
    }

    public enum FeedbackVerificationResult
    {
        Ok = 0,
        SessionIdEmpty = 1,
        UserIdEmpty = 2,
        UserAlreadyHasFeedback = 3,
        RatingInappropriate = 4
    }
}
