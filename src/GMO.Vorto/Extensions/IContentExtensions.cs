using Newtonsoft.Json;
using Our.Umbraco.Vorto.Exceptions;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace GMO.Vorto.Extensions
{
    /// <summary>
    /// <see cref="IContent"/> extension methods
    /// </summary>
    public static class IContentExtensions
    {
        /// <summary>
        /// Sets vorto values for all languages in provided dictionary object
        /// </summary>
        /// <param name="content"></param>
        /// <param name="alias">Property alias</param>
        /// <param name="items">Dictionary containing all vorto values for all languages used by property</param>
        /// <exception cref="VortoException">Throws VortoException when it cannot find matching property from alias</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetVortoValue(this IContent content, string alias, Dictionary<string, object> items)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
            if (content == items) throw new ArgumentNullException(nameof(items));

            var property = content.Properties.FirstOrDefault(x => x.Alias == alias);

            if (property != null)
            {
                var vortoValue = new VortoValue
                {
                    DtdGuid = property.Key,
                    Values = items
                };

                var json = JsonConvert.SerializeObject(vortoValue);

                content.SetValue(alias, json);
            }

            throw new VortoException("Unable to find matching property on IContent.");
        }
    }
}
