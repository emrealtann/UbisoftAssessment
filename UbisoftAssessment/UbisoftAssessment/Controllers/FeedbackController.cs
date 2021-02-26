using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Entities.Dto;
using UbisoftAssessment.Repositories.Interfaces;
using UbisoftAssessment.Resources;

namespace UbisoftAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _repository;
        private readonly ILogger<FeedbackController> _logger;
        private readonly CommonLocalizationService _localizer;

        public FeedbackController(IFeedbackRepository repository, ILogger<FeedbackController> logger, CommonLocalizationService localizer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks([FromQuery] int? rating = null)
        {
            var feedbacks = await _repository.GetFeedbacks(rating);
            return Ok(feedbacks);
        }

        [Route("[action]/{sessionId}")]
        [HttpPost]
        [ProducesResponseType(typeof(Feedback), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Feedback>> CreateFeedback([FromHeader(Name= "Ubi-UserId")] string userId, [FromRoute] string sessionId, [FromBody] FeedbackDto dto)
        {
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return BadRequest("Please specify the Ubi-UserId in the request header.");
            //}
            //else if (string.IsNullOrEmpty(sessionId))
            //{
            //    return BadRequest("Please specify the session id in the request header.");
            //}

            Feedback feedback = new Feedback()
            {
                SessionId = sessionId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedOn = DateTime.UtcNow
            };

            FeedbackVerificationResult verificationResult = await _repository.VerifyFeedback(feedback);
            if (verificationResult != FeedbackVerificationResult.Ok)
            {
                string errorMessage = _localizer.Get(verificationResult.ToString());
                return BadRequest(errorMessage);
            }

            await _repository.CreateFeedback(feedback);

            return Ok(feedback);
        }
    }
}
