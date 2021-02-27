using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UbisoftAssessment.Services.Interfaces
{
    public interface ICommonLocalizationService
    {
        public string Get(string key);
    }
}
