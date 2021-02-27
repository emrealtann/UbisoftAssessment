using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Data.Interfaces
{
    /// <summary>
    /// MongoDB context interface for the feedbacks.
    /// </summary>
    public interface IFeedbackContext
    {
        /// <summary>
        /// Feedback collection object 
        /// </summary>
        IMongoCollection<Feedback> Feedbacks { get; }
    }
}
