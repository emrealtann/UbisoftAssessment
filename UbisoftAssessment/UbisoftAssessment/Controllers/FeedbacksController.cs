using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UbisoftAssessment.Entities;
using UbisoftAssessment.Entities.Dto;
using UbisoftAssessment.Services.Interfaces;
using UbisoftAssessment.Services;

namespace UbisoftAssessment.Controllers
{
    /// <summary>
    /// Feedback API controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _service;
        private readonly ILogger<FeedbacksController> _logger;
        private readonly CommonLocalizationService _localizer;

        /// <summary>
        /// Constructor method for the API controller
        /// </summary>
        public FeedbacksController(IFeedbackService service, ILogger<FeedbacksController> logger, CommonLocalizationService localizer)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localizer = localizer;
        }

        /// <summary>
        /// Get the feedback list
        /// </summary>
        /// <remarks>
        /// Lists the last 15 feedbacks left by players and allows filtering by rating.
        /// </remarks>
        /// <param name="rating">Rating filter. Can be between 1 and 5.</param>
        /// <returns>Feedback list.</returns>
        /// <response code="200">Returns the feedback list.</response>
        /// <response code="400">There is an error. Returns error message.</response>  
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Feedback>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks([FromQuery] int? rating = null)
        {
            try
            {
                var feedbacks = await _service.GetFeedbacks(rating);
                return Ok(feedbacks);
            }
            catch(Exception ex)
            {
                //try to get error message from the localizer service. 
                //it will return the exc message if it cannot find the key in the resource.
                string message = _localizer.Get(ex.Message);

                _logger.LogError(ex, "GetFeedbacks Error: " + message);
                return BadRequest(message);
            }
        }

        /// <summary>
        /// Create a feedback
        /// </summary>
        /// <remarks>
        /// Creates a new feedback with given parameters, returns the inserted feedback.
        /// </remarks>
        /// <param name="userId">Ubisoft user ID. Should be defined in the request header named Ubi-UserId.</param>
        /// <param name="sessionId">Ubisoft session ID. Should be defined in the url path.</param>
        /// <param name="dto">Request payload. User rating should be from 1 to 5.</param>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">There is an error. Returns error message.</response>  
        [Route("{sessionId}")]
        [HttpPost]
        [ProducesResponseType(typeof(Feedback), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Feedback>> CreateFeedback([FromHeader(Name= "Ubi-UserId")] string userId, [FromRoute] string sessionId, [FromBody] FeedbackCreateDto dto)
        {
            try
            {
                Feedback feedback = new Feedback()
                {
                    SessionId = sessionId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedOn = DateTime.UtcNow
                };

                FeedbackVerificationResult verificationResult = await _service.VerifyFeedback(feedback);
                if (verificationResult != FeedbackVerificationResult.Ok)
                {
                    string errorMessage = _localizer.Get(verificationResult.ToString());
                    return BadRequest(errorMessage);
                }

                await _service.CreateFeedback(feedback);

                return Ok(feedback);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateFeedback Error");
                return BadRequest("Error Occurred: " + ex.Message);
            }
        }
    }
}
