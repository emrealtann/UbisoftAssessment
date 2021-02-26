using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UbisoftAssessment.Entities;

namespace UbisoftAssessment.Data.Interfaces
{
    public interface IFeedbackContext
    {
        IMongoCollection<Feedback> Feedbacks { get; }
    }
}
