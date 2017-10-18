using Our.Umbraco.Vorto.Extensions;
using Our.Umbraco.Vorto.Models;
using System.Configuration;

namespace Our.Umbraco.Vorto
{
    public static class Settings
    {
        internal static ILanguageRetriever customRetriever;

        public static bool DetailedLogging => ConfigurationManager.AppSettings["Vorto.DetailedLogging"].ConvertToBool();
    }
}
