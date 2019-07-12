using GMO.Vorto.PropertyEditor;
using Our.Umbraco.Vorto.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Language = Our.Umbraco.Vorto.Models.Language;

namespace Our.Umbraco.Vorto.Web.Controllers
{
    [PluginController("VortoApi")]
    public class VortoApiController : UmbracoAuthorizedJsonController
    {
        public IEnumerable<object> GetNonVortoDataTypes()
        {
            return Services.DataTypeService.GetAll()
                .Where(x => x.EditorAlias != "Our.Umbraco.Vorto")
                .OrderBy(x => x.SortOrder)
                .Select(x => new
                {
                    guid = x.Key,
                    name = x.Name,
                    propertyEditorAlias = x.EditorAlias
                });
        }

        public object GetDataTypeById(Guid id)
        {
            var dtd = Services.DataTypeService.GetDataType(id);
            return FormatDataType(dtd);
        }

        public object GetDataTypeByAlias(string contentType, string contentTypeAlias, string propertyAlias)
        {
            IContentTypeComposition ct = null;

            switch (contentType)
            {
                case "member":
                    ct = Services.MemberTypeService.Get(contentTypeAlias);
                    break;
                case "content":
                    ct = Services.ContentTypeService.Get(contentTypeAlias);
                    break;
                case "media":
                    ct = Services.MediaTypeService.Get(contentTypeAlias);
                    break;
            }

            if (ct == null)
                return null;

            var prop = ct.CompositionPropertyTypes.SingleOrDefault(x => x.Alias == propertyAlias);
            if (prop == null)
                return null;

            var dtd = Services.DataTypeService.GetDataType(prop.DataTypeKey);
            return FormatDataType(dtd);
        }

        protected object FormatDataType(IDataType dtd)
        {
            if (dtd == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var propEditor = dtd.Editor;

            var preValues = dtd.Configuration;

            var convertedPreValues = propEditor.GetConfigurationEditor()
                .ToConfigurationEditor(preValues);

            return new
            {
                guid = dtd.Key,
                propertyEditorAlias = dtd.EditorAlias,
                preValues = convertedPreValues,
                view = propEditor.GetValueEditor().View,
            };
        }

        public IEnumerable<Language> GetLanguages(string section, int id, int parentId, Guid dtdGuid)
        {
            var dtd = Services.DataTypeService.GetDataType(dtdGuid);
            if (dtd == null) return Enumerable.Empty<Language>();

            var preValues = (VortoConfiguration) dtd.Configuration;
            var languageSource = preValues?.LanguageSource ?? "";
            var primaryLanguage = preValues?.PrimaryLanguage ?? "";

            var languages = new List<Language>();

            if (languageSource == "inuse")
            {
                var xpath = preValues?.XPath ?? "";

                throw new NotImplementedException();

                //// Grab languages by xpath (only if in content section)
                //if (!string.IsNullOrWhiteSpace(xpath) && section == "content")
                //{
                //    xpath = xpath.Replace("$currentPage",
                //        string.Format("//*[@id={0} and @isDoc]", id)).Replace("$parentPage",
                //            string.Format("//*[@id={0} and @isDoc]", parentId)).Replace("$ancestorOrSelf",
                //                string.Format("//*[@id={0} and @isDoc]", id != 0 ? id : parentId));

                //    // Lookup language nodes
                //    var nodeIds = uQuery.GetNodesByXPath(xpath).Select(x => x.Id).ToArray();
                //    if (nodeIds.Any())
                //    {
                //        var db = ApplicationContext.Current.DatabaseContext.Database;
                //        languages.AddRange(db.Query<string>(
                //            string.Format(
                //                "SELECT DISTINCT [languageISOCode] FROM [umbracoLanguage] JOIN [umbracoDomains] ON [umbracoDomains].[domainDefaultLanguage] = [umbracoLanguage].[id] WHERE [umbracoDomains].[domainRootStructureID] in ({0})",
                //                string.Join(",", nodeIds)))
                //            .Select(CultureInfo.GetCultureInfo)
                //            .Select(x => new Language
                //            {
                //                IsoCode = x.Name,
                //                Name = x.DisplayName,
                //                NativeName = x.NativeName,
                //                IsRightToLeft = x.TextInfo.IsRightToLeft
                //            }));
                //    }
                //}
                //else
                //{
                //    // No language node xpath so just return a list of all languages in use
                //    var db = ApplicationContext.Current.DatabaseContext.Database;
                //    languages.AddRange(
                //        db.Query<string>(
                //            "SELECT [languageISOCode] FROM [umbracoLanguage] WHERE EXISTS(SELECT 1 FROM [umbracoDomains] WHERE [umbracoDomains].[domainDefaultLanguage] = [umbracoLanguage].[id])")
                //            .Select(CultureInfo.GetCultureInfo)
                //            .Select(x => new Language
                //            {
                //                IsoCode = x.Name,
                //                Name = x.DisplayName,
                //                NativeName = x.NativeName,
                //                IsRightToLeft = x.TextInfo.IsRightToLeft
                //            }));
                //}
            }
            else if (languageSource == "custom")
            {
                if (Settings.customRetriever != null)
                {
                    Logger.Debug<VortoApiController>("About to use custom retriever");

                    languages.AddRange(Settings.customRetriever.GetLanguages());
                }
                else
                {
                    throw new VortoException("No ILanguageRetriever found for use with PropertyEditor with custom language source.");
                }
            }
            else
            {
                languages.AddRange(GetInstalledLanguages());
            }

            // Raise event to allow for further filtering
            var args = new FilterLanguagesEventArgs
            {
                CurrentPageId = id,
                ParentPageId = parentId,
                Languages = languages
            };

            Vorto.CallFilterLanguages(args);

            // Set active language
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

            // See if one has already been set via the event handler
            var activeLanguage = args.Languages.FirstOrDefault(x => x.IsDefault);

            // Try setting to primary language
            if (activeLanguage == null && !string.IsNullOrEmpty(primaryLanguage))
                activeLanguage = args.Languages.FirstOrDefault(x => x.IsoCode == primaryLanguage);

            // Try settings to exact match of current culture
            if (activeLanguage == null)
                activeLanguage = args.Languages.FirstOrDefault(x => x.IsoCode == currentCulture);

            // Try setting to nearest match
            if (activeLanguage == null)
                activeLanguage = args.Languages.FirstOrDefault(x => x.IsoCode.Contains(currentCulture));

            // Try setting to nearest match
            if (activeLanguage == null)
                activeLanguage = args.Languages.FirstOrDefault(x => currentCulture.Contains(x.IsoCode));

            // Couldn't find a good enough match, just select the first language
            if (activeLanguage == null)
                activeLanguage = args.Languages.FirstOrDefault();

            if (activeLanguage != null)
                activeLanguage.IsDefault = true;

            // Return results
            return args.Languages;
        }

        public IEnumerable<Language> GetInstalledLanguages()
        {
            return Services.LocalizationService.GetAllLanguages()
                .Select(x => CultureInfo.GetCultureInfo(x.IsoCode))
                .Select(x => new Language
                {
                    IsoCode = x.Name,
                    Name = x.DisplayName,
                    NativeName = x.NativeName,
                    IsRightToLeft = x.TextInfo.IsRightToLeft
                });

            //return new List<Language>
            //{
            //    new Language
            //    {
            //        IsoCode = "EU",
            //        Name = "Europe",
            //        NativeName = "Europe",
            //        IsDefault = true,
            //    },
            //    new Language
            //    {
            //        IsoCode = "is-IS",
            //        Name = "Iceland",
            //        NativeName = "Ísland"
            //    }
            //};
        }
    }
}
