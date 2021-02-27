using System;

namespace UbisoftAssessment.Entities.Dto
{
    /// <summary>
    /// Data transfer object to create feedbacks.
    /// </summary>
    public class FeedbackCreateDto
    {
        /// <summary>
        /// User rating for the game session. Must be between 1 and 5.
        /// </summary>
        /// <example>5</example>
        public int Rating { get; set; }

        /// <summary>
        /// User comment for the game session.
        /// </summary>
        /// <example>That was a perfect experience!</example>
        public string Comment { get; set; }
    }
}
