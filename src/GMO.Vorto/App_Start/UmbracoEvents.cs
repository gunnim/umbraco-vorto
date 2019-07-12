using Our.Umbraco.Vorto.Exceptions;
using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Logging;
using Umbraco.Core.Composing;

namespace Our.Umbraco.Vorto.App_Start
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    class Startup : IUserComposer
    {
        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        public void Compose(Composition composition)
        {
            var ppType = typeof(ILanguageRetriever);
            var languageRetrievers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType))
                .Where(t => !t.IsInterface);

            if (languageRetrievers.Any())
            {
                composition.Logger.Debug<Startup>("Found language retriever[s].");

                Type langRetriever;

                try
                {
                    langRetriever = languageRetrievers.Single();
                }
                catch (InvalidOperationException)
                {
                    throw new VortoException(
                        "More than one ILanguageRetriever found. Currently, only one custom language retriever is supported."
                    );
                }

                Settings.customRetriever = Activator.CreateInstance(langRetriever) as ILanguageRetriever;

                composition.Logger.Debug<Startup>("Settings.customRetriever == " + Settings.customRetriever);
            }
        }
    }
}
