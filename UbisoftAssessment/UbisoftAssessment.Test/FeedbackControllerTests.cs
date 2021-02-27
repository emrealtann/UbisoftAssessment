using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using UbisoftAssessment.Controllers;
using UbisoftAssessment.Data;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Entities.Dto;
using UbisoftAssessment.Services;
using UbisoftAssessment.Services.Interfaces;
using Xunit;

namespace UbisoftAssessment.Test
{
    public class FeedbackControllerTests
    {
        private FeedbacksController controller;
        private Mock<IFeedbackService> feedbackServiceMock;
        private Mock<ILogger<FeedbacksController>> loggerMock;
        private Mock<ICommonLocalizationService> localizerMock;
        private List<Feedback> items;
        
        public FeedbackControllerTests()
        {
            feedbackServiceMock = new Mock<IFeedbackService>();
            loggerMock = new Mock<ILogger<FeedbacksController>>();
            localizerMock = new Mock<ICommonLocalizationService>();

            items = FeedbackContextSeed.GetPreconfiguredFeedbacks().ToList();
            controller = new FeedbacksController(feedbackServiceMock.Object, loggerMock.Object, localizerMock.Object);
        }

        [Fact]
        public async void GetFeedbacks_WithoutFilter_Ok()
        {
            feedbackServiceMock.Setup(x => x.GetFeedbacks(null)).ReturnsAsync(items);
            var result = await controller.GetFeedbacks();

            feedbackServiceMock.Verify(x => x.GetFeedbacks(null), Times.Once);
            var resultType = Assert.IsType<OkObjectResult>(result.Result);
            var resultModel = Assert.IsAssignableFrom<List<Feedback>>(resultType.Value);
        }

        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [Theory]
        public async void GetFeedbacks_WithFilter_Ok(int rating)
        {
            var list = items.Where(x => x.Rating == rating).ToList();
            feedbackServiceMock.Setup(x => x.GetFeedbacks(rating)).ReturnsAsync(list);
            var result = await controller.GetFeedbacks(rating);

            feedbackServiceMock.Verify(x => x.GetFeedbacks(rating), Times.Once);
            var resultType = Assert.IsType<OkObjectResult>(result.Result);
            var resultModel = Assert.IsAssignableFrom<List<Feedback>>(resultType.Value);
        }

        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(6)]
        [Theory]
        public async void GetFeedbacks_WithInvalidFilter_BadRequest(int rating)
        {
            feedbackServiceMock.Setup(x => x.GetFeedbacks(rating))
                .ThrowsAsync(new Exception(FeedbackVerificationResult.RatingInappropriate.ToString()));

            var result = await controller.GetFeedbacks(rating);

            feedbackServiceMock.Verify(x => x.GetFeedbacks(rating), Times.Once);
            var resultType = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, resultType.StatusCode);
        }

        [Fact]
        public async void CreateFeedback_WithNullSessionId_BadRequest()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 1,
                SessionId = null,
                UserId = "test"
            };

            FeedbackCreateDto dto = new FeedbackCreateDto()
            {
                Comment = feedback.Comment,
                Rating = feedback.Rating
            };

            feedbackServiceMock.Setup(x => x.VerifyFeedback(It.IsAny<Feedback>()))
                .ReturnsAsync(FeedbackVerificationResult.SessionIdEmpty);

            var result = await controller.CreateFeedback(feedback.UserId, feedback.SessionId, dto);

            feedbackServiceMock.Verify(x => x.CreateFeedback(feedback), Times.Never);
            var resultType = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, resultType.StatusCode);
        }

        [Fact]
        public async void CreateFeedback_WithNullUserId_BadRequest()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 1,
                SessionId = "test",
                UserId = null
            };

            FeedbackCreateDto dto = new FeedbackCreateDto()
            {
                Comment = feedback.Comment,
                Rating = feedback.Rating
            };

            feedbackServiceMock.Setup(x => x.VerifyFeedback(It.IsAny<Feedback>()))
                .ReturnsAsync(FeedbackVerificationResult.UserIdEmpty);

            var result = await controller.CreateFeedback(feedback.UserId, feedback.SessionId, dto);

            feedbackServiceMock.Verify(x => x.CreateFeedback(feedback), Times.Never);
            var resultType = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, resultType.StatusCode);
        }

        [Fact]
        public async void CreateFeedback_WithIncorrectRating_BadRequest()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 7,
                SessionId = "test",
                UserId = "test"
            };

            FeedbackCreateDto dto = new FeedbackCreateDto()
            {
                Comment = feedback.Comment,
                Rating = feedback.Rating
            };

            feedbackServiceMock.Setup(x => x.VerifyFeedback(It.IsAny<Feedback>()))
                .ReturnsAsync(FeedbackVerificationResult.RatingInappropriate);

            var result = await controller.CreateFeedback(feedback.UserId, feedback.SessionId, dto);

            feedbackServiceMock.Verify(x => x.CreateFeedback(feedback), Times.Never);
            var resultType = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, resultType.StatusCode);
        }

        [Fact]
        public async void CreateFeedback_WithExistingFeedback_BadRequest()
        {
            Feedback feedback = items.First();
            feedback.Id = null;

            FeedbackCreateDto dto = new FeedbackCreateDto()
            {
                Comment = feedback.Comment,
                Rating = feedback.Rating
            };

            feedbackServiceMock.Setup(x => x.VerifyFeedback(It.IsAny<Feedback>()))
                .ReturnsAsync(FeedbackVerificationResult.UserAlreadyHasFeedback);

            var result = await controller.CreateFeedback(feedback.UserId, feedback.SessionId, dto);

            feedbackServiceMock.Verify(x => x.CreateFeedback(feedback), Times.Never);
            var resultType = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, resultType.StatusCode);
        }

        [Fact]
        public async void CreateFeedback_Ok()
        {
            Feedback feedback = new Feedback
            {
                Comment = "test",
                CreatedOn = DateTime.UtcNow,
                Rating = 5,
                SessionId = "10",
                UserId = "1"
            };

            FeedbackCreateDto dto = new FeedbackCreateDto()
            {
                Comment = feedback.Comment,
                Rating = feedback.Rating
            };

            feedbackServiceMock.Setup(x => x.VerifyFeedback(It.IsAny<Feedback>()))
                .ReturnsAsync(FeedbackVerificationResult.Ok);

            feedbackServiceMock.Setup(x => x.CreateFeedback(It.IsAny<Feedback>()));

            var result = await controller.CreateFeedback(feedback.UserId, feedback.SessionId, dto);

            feedbackServiceMock.VerifyAll();
            var resultType = Assert.IsType<OkObjectResult>(result.Result);
            var resultModel = Assert.IsAssignableFrom<Feedback>(resultType.Value);
        }
    }
}
