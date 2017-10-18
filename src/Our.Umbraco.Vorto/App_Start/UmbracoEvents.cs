using log4net;
using Our.Umbraco.Vorto.Exceptions;
using Our.Umbraco.Vorto.Helpers;
using Our.Umbraco.Vorto.Models;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;

namespace Our.Umbraco.Vorto.App_Start
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    class UmbracoEvents : ApplicationEventHandler
    {
        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );

        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var ppType = typeof(ILanguageRetriever);
            var languageRetrievers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType));

            if (languageRetrievers.Any())
            {
                //if (Settings.DetailedLogging)
                //{
                //    Log.Info("Found language retriever[s].");
                //}

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

                //if (Settings.DetailedLogging)
                //{
                //    Log.Info("Settings.customRetriever == " + Settings.customRetriever);
                //}
            }
        }
    }
}
