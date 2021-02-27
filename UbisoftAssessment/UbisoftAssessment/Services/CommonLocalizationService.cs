using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UbisoftAssessment.Resources;

namespace UbisoftAssessment.Services
{
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;
        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(SharedResource), assemblyName.Name);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
