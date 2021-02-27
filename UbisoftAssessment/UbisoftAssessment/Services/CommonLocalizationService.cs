using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UbisoftAssessment.Resources;

namespace UbisoftAssessment.Services
{
    /// <summary>
    /// Localization service. Uses the default localization resource or the current culture resource.
    /// </summary>
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;

        /// <summary>
        /// Constructor method for the localization service.
        /// </summary>
        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(SharedResource), assemblyName.Name);
        }

        /// <summary>
        /// Gets the value by key from the localization resource file.
        /// </summary>
        /// <param name="key">Key of the resource.</param>
        /// <returns>Localized string value of the resource.</returns>
        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
