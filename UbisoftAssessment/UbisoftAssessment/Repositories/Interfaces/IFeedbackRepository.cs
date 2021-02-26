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
        Task<IEnumerable<Feedback>> GetFeedbacks(int rating);
    }
}
