using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbisoftAssessment.Data;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Services;
using UbisoftAssessment.Services.Interfaces;
using Xunit;

namespace UbisoftAssessment.Test
{
    public class FeedbackServiceTests
    {
        private FeedbackService service;
        public FeedbackServiceTests()
        {
            //using same mongodb instance. normally there should be a seperate instance for testing purposes.
            var myConfiguration = new Dictionary<string, string>
            {
                {"DatabaseSettings:ConnectionString", "mongodb+srv://dbUbi:bUYuAF5dKFWzdfRE@feedback.a4bpz.mongodb.net"},
                {"DatabaseSettings:DatabaseName", "CustomerFeedback"},
                {"DatabaseSettings:CollectionName", "Feedbacks"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            FeedbackContext feedbackContext = new FeedbackContext(configuration);
            service = new FeedbackService(feedbackContext);
        }

        [Fact]
        public async void GetFeedbacks_WithoutFilter_Ok()
        {
            var feedbacks = await service.GetFeedbacks();

            //the feedbacks list shouldn't be null or empty. because seed method is running on startup.
            Assert.NotNull(feedbacks);
            Assert.NotEmpty(feedbacks);

            //should return last 15 feedbacks so size of the list should be less than 16
            Assert.True(feedbacks.Count() <= 15);

            //it should be sorted by creation dates in descending order
            var firstFeedbackId = feedbacks.First().Id;
            var mostRecentId = feedbacks.OrderByDescending(x => x.CreatedOn).First().Id;
            Assert.Equal(mostRecentId, firstFeedbackId);
        }

        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [Theory]
        public async void GetFeedbacks_WithFilter_Ok(int rating)
        {
            var feedbacks = await service.GetFeedbacks(rating);
            Assert.NotNull(feedbacks);
            Assert.NotEmpty(feedbacks);

            //there shouldn't be a feedback with another rating value.
            bool hasOtherRatings = feedbacks.Any(x => x.Rating != rating);
            Assert.False(hasOtherRatings);
        }

        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(6)]
        [Theory]
        public async void GetFeedbacks_WithInvalidFilter_Exception(int rating)
        {
            //it should throw an exception for these inappropriate rating values
            await Assert.ThrowsAsync<Exception>(() => service.GetFeedbacks(rating));
        }

        [Fact]
        public async void VerifyFeedback_WithNullSessionId_Exception()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 1,
                SessionId = null,
                UserId = "test"
            };

            //it should give SessionIdEmpty result
            var result = await service.VerifyFeedback(feedback);
            Assert.Equal(FeedbackVerificationResult.SessionIdEmpty, result);
        }

        [Fact]
        public async void VerifyFeedback_WithNullUserId_Exception()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 1,
                SessionId = "test",
                UserId = null
            };

            //it should give UserIdEmpty result
            var result = await service.VerifyFeedback(feedback);
            Assert.Equal(FeedbackVerificationResult.UserIdEmpty, result);
        }

        [Fact]
        public async void VerifyFeedback_WithIncorrectRating_Exception()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 7,
                SessionId = "test",
                UserId = "test"
            };

            //it should give RatingInappropriate result
            var result = await service.VerifyFeedback(feedback);
            Assert.Equal(FeedbackVerificationResult.RatingInappropriate, result);
        }

        [Fact]
        public async void VerifyFeedback_WithExistingFeedback_Exception()
        {
            var feedbacks = await service.GetFeedbacks();
            Feedback feedback = feedbacks.First();
            feedback.Id = null;

            //it should give UserAlreadyHasFeedback result (a player can only leave one feedback per session)
            var result = await service.VerifyFeedback(feedback);
            Assert.Equal(FeedbackVerificationResult.UserAlreadyHasFeedback, result);
        }

        [Fact]
        public async void VerifyFeedback_Ok()
        {
            var feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 5,
                SessionId = "1",
                UserId = Guid.NewGuid().ToString()
            };

            //it should return ok
            var result = await service.VerifyFeedback(feedback);
            Assert.Equal(FeedbackVerificationResult.Ok, result);
        }
    }
}
