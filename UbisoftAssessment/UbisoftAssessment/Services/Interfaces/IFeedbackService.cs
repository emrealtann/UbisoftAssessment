using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Services.Interfaces
{
    /// <summary>
    /// Repository interface for the feedback collection. Can be used for getting feedbacks or CRUD operations.
    /// </summary>
    public interface IFeedbackService
    {
        /// <summary>
        /// Creates a feedback in the MongoDB feedback collection.
        /// </summary>
        /// <param name="feedback">Feedback entity to create.</param>
        Task CreateFeedback(Feedback feedback);

        /// <summary>
        /// Lists the last 15 feedbacks left by players and allows filtering by rating.
        /// </summary>
        /// <param name="rating">Rating filter. Can be between 1 and 5.</param>
        /// <returns>Feedback list.</returns>
        Task<IEnumerable<Feedback>> GetFeedbacks(int? rating = null);

        /// <summary>
        /// Verifies the feedback to be inserted.
        /// </summary>
        /// <param name="feedback">Feedback entity to be inserted.</param>
        /// <returns>
        /// Returns FeedbackVerificationResult enum. Returns Ok if everything is ok.
        /// If it doesn't return ok, then there is a problem with the entity values.
        /// You can get the error message with the returned enum from the localization resource.
        /// </returns>
        Task<FeedbackVerificationResult> VerifyFeedback(Feedback feedback);
    }

    /// <summary>
    /// Feedback verification result enum. Represents the error message keys if there is something wrong about verification.
    /// </summary>
    public enum FeedbackVerificationResult
    {
        /// <summary>
        /// Means that everything is ok.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Means that feedback session ID is null or empty
        /// </summary>
        SessionIdEmpty = 1,

        /// <summary>
        /// Means that feedback user ID is null or empty
        /// </summary>
        UserIdEmpty = 2,

        /// <summary>
        /// Means that user already has a feedback for the session.
        /// </summary>
        UserAlreadyHasFeedback = 3,

        /// <summary>
        /// Means that rating is not between 1 and 5.
        /// </summary>
        RatingInappropriate = 4
    }
}
