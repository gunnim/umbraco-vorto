using System.Collections.Generic;

namespace Our.Umbraco.Vorto.Models
{
    public interface ILanguageRetriever
    {
        IEnumerable<Language> GetLanguages();
    }
}
