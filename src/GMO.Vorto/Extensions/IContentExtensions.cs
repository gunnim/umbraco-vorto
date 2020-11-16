using Newtonsoft.Json;
using Our.Umbraco.Vorto.Exceptions;
using Our.Umbraco.Vorto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;

namespace Our.Umbraco.Vorto.Extensions
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

            var property = content.Properties.FirstOrDefault(x => x.Alias.ToUpperInvariant() == alias.ToUpperInvariant());

            if (property != null)
            {
                var dts = Current.Services.DataTypeService;

                var dt = dts.GetByEditorAlias(property.PropertyType.PropertyEditorAlias);

                if (dt.Any())
                {
                    var dtt = dt.FirstOrDefault();

                    var vortoValue = new VortoValue();
                    vortoValue.DtdGuid = dtt.Key;
                    vortoValue.Values = items;

                    var json = JsonConvert.SerializeObject(vortoValue);

                    content.SetValue(alias, json);
                    return;
                }

                throw new InvalidOperationException("Unable to get data type for property.");
            }

            throw new InvalidOperationException("Unable to find matching property on IContent.");
        }

        public static void SetVortoValue(this IContent content, string alias, string storeAlias, object value)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
            if (string.IsNullOrEmpty(storeAlias)) throw new ArgumentNullException(nameof(storeAlias));

            var property = content.Properties.FirstOrDefault(x => x.Alias.ToUpperInvariant() == alias.ToUpperInvariant());

            if (property != null)
            {
                var vortoValue = content.GetVortoObject(alias);

                var vortoItems = new Dictionary<string, object>();

                if (vortoValue != null && vortoValue.Values != null && vortoValue.Values.Any())
                {
                    foreach (var vvalue in vortoValue.Values)
                    {
                        var val = vvalue.Key.ToUpperInvariant() == storeAlias.ToUpperInvariant() ? value : vvalue.Value;

                        vortoItems.Add(vvalue.Key, val);
                    }
                }
                else
                {
                    vortoItems.Add(storeAlias, value);
                }

                SetVortoValue(content, alias, vortoItems);
                return;
            }

            throw new InvalidOperationException("Unable to find matching property on IContent.");
        }

        public static object GetVortoValue(this IContent content, string alias, string cultureName)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new ArgumentNullException(nameof(cultureName));
            }

            var vortoModel = GetVortoObject(content, alias);

            if (vortoModel?.Values != null)
            {
                var bestMatchCultureName = vortoModel.FindBestMatchCulture(cultureName);
                if (!bestMatchCultureName.IsNullOrWhiteSpace()
                    && vortoModel.Values.ContainsKey(bestMatchCultureName)
                    && vortoModel.Values[bestMatchCultureName] != null
                    && vortoModel.Values[bestMatchCultureName]?.ToString().IsNullOrWhiteSpace() == false)
                {
                    return vortoModel.Values[bestMatchCultureName];
                }
            }
             
            return default;
        }

        public static VortoValue GetVortoObject(this IContent content, string alias)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var property = content.Properties.FirstOrDefault(x => x.Alias.ToUpperInvariant() == alias.ToUpperInvariant());

            if (property == null)
            {
                throw new InvalidOperationException("Unable to find matching property on IContent.");
            }

            var val = property.GetValue();
            if (val != null)
            {
                return JsonConvert.DeserializeObject<VortoValue>(val.ToString());
            }

            return null;
        }
    }
}
