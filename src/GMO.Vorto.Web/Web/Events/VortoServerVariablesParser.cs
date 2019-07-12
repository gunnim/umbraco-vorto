using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using System.Web.Routing;
using System.Web.Mvc;
using Our.Umbraco.Vorto.Web.Controllers;
using Umbraco.Web;
using Umbraco.Web.JavaScript;
using Umbraco.Core.Composing;

namespace Our.Umbraco.Vorto.Web.Events
{
    public class VortoServerVariablesParser : IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }
        public void Terminate()
        {
        }

        void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) return;
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var mainDictionary = new Dictionary<string, object>
            {
                {
                    "apiBaseUrl",
                    urlHelper.GetUmbracoApiServiceBaseUrl<VortoApiController>(
                        controller => controller.GetInstalledLanguages())
                }
            };

            if (!e.Keys.Contains("vorto"))
            {
                e.Add("vorto", mainDictionary);
            }
        }
    }
}
