using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RazorEngine.Configuration;
using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Text;
using System.Reflection;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web;
using System.Web.Mvc.Html;

namespace PitchingTube.Mailing
{
    public class EmailParser
    {
        private const string Namespace = "PitchingTube.Mailing.Variables.";
        private const string Extension = ".cshtml";

        public static string Parse<T>(string templateID, string template, T model, bool wrap, bool encode = true)
        {
            var variables = GetVariables(templateID);
            var pattern = "(" + string.Join("|", variables.Select(Regex.Escape)) + ")";

            var templateConfig = new TemplateServiceConfiguration
                                     {
                                         EncodedStringFactory = encode
                                                ? (IEncodedStringFactory)
                                                    new HtmlEncodedStringFactory()
                                                : new RawStringFactory()
                                     };
            templateConfig.BaseTemplateType = typeof(HtmlTemplateBase<>);

            var templateService = new TemplateService(templateConfig);

            Razor.SetTemplateService(templateService);

            string text = Regex.Replace(template, pattern, match =>
                {
                    var variable = match.Groups[1].Value;
                    var templateName = variable.Trim('#');

                    string variableTemplate = GetVariableTemplate(templateID, templateName);
                    return templateService.Parse(variableTemplate, model);
                });

            if (wrap)
            {
                string wrapTemplate = GetVariableTemplate(templateID, "_Wrap");
                if (wrapTemplate != null)
                {
                    return templateService.Parse<string>(wrapTemplate, text);
                }
            }
            return text;
        }

        public static string[] GetVariables(string templateID)
        {
            string start = Namespace + templateID + ".";

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(n => n.StartsWith(start) && n.EndsWith(Extension))
                .Select(n => n.Substring(start.Length, n.Length - start.Length - Extension.Length))
                .Where(n => !n.StartsWith("_"))
                .Select(FormatVariable);
            
            return names.ToArray();
        }

        private static string FormatVariable(string variable)
        {
            return string.Format("#{0}#", variable);
        }

        private static string GetVariableTemplate(string parentTemplateID, string templateName)
        {
            string start = Namespace + parentTemplateID + ".";
            string resourceName = start + templateName + Extension;
            var assembly = Assembly.GetExecutingAssembly();
            if (!assembly.GetManifestResourceNames().Contains(resourceName)) { return null; }
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd().Trim();
            }
        }
    }

    [RequireNamespaces("System.Web.Mvc.Html")]
    public class HtmlTemplateBase<T> : TemplateBase<T>, IViewDataContainer
    {
        private HtmlHelper<T> helper = null;
        private ViewDataDictionary viewdata = null;
        private System.Dynamic.DynamicObject viewbag = null;


        public dynamic ViewBag
        {
            get
            {
                return (WebPageContext.Current.Page as WebViewPage).ViewBag;
            }
        }

        public HtmlHelper<T> Html
        {
            get
            {
                if (helper == null)
                {
                    var p = WebPageContext.Current;
                    var wvp = p.Page as WebViewPage;
                    var context = wvp != null ? wvp.ViewContext : null;
                    helper = new HtmlHelper<T>(context, this);
                }
                return helper;
            }
        }

        public void RenderPatial(string name, object model)
        {
            Html.RenderPartial(name, model);
        }

        public HtmlString ValueOrMinus(object value, string formatString = null)
        {
            return ValueOrPlaceHolder( "-", value);
        }

        private HtmlString ValueOrPlaceHolder(string placeHolder, object value,
                                                     string formatString = null)
        {
            if (value == null || value.ToString().Trim().Length == 0)
            {
                return new HtmlString(placeHolder);
            }
            if (formatString != null)
            {
                return new HtmlString(string.Format(formatString, value));
            }
            return new HtmlString(value.ToString());
        }

        public ViewDataDictionary ViewData
        {
            get
            {
                if (viewbag == null)
                {
                    var p = WebPageContext.Current;
                    var viewcontainer = p.Page as IViewDataContainer;
                    viewdata = new ViewDataDictionary(viewcontainer.ViewData);

                    if (this.Model != null)
                    {
                        viewdata.Model = Model;
                    }
                }

                return viewdata;
            }
            set
            {
                viewdata = value;
            }
        }
    }
}
