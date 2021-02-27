using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UbisoftAssessment.Entities
{
    /// <summary>
    /// Feedback entity. Contains user feedbacks for the game sessions.
    /// </summary>
    public class Feedback
    {
        /// <summary>
        /// Unique MongoDB record ID of the feedback.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Game session id
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Ubi-UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User rating for the game session. Should be between 1 and 5.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// User comment about the game session.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Feedback creation date and time in UTC format.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
