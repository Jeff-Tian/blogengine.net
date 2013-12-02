using System;
using System.Text;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;

namespace BlogEngine.Core.Web.Scripting
{
    /// <summary>
    /// Helper methods for script manipulations
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Add stylesheet to page header
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="lnk">Href link</param>
        public static void AddStyle(System.Web.UI.Page page, string lnk)
        {
            // global styles on top, before theme specific styles
            if(lnk.Contains("Global.css") || lnk.Contains("Styles/css"))
                page.Header.Controls.AddAt(0, new LiteralControl(
                string.Format("\n<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", lnk)));
            else
                page.Header.Controls.Add(new LiteralControl(
                string.Format("\n<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", lnk)));
        }
        /// <summary>
        /// Add generic lit to the page
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="type">Type</param>
        /// <param name="relation">Relation</param>
        /// <param name="title">Title</param>
        /// <param name="href">Url</param>
        public static void AddGenericLink(System.Web.UI.Page page, string type, string relation, string title, string href)
        {
            var tp = string.IsNullOrEmpty(type) ? "" : string.Format("type=\"{0}\" ", type);
            const string tag = "\n<link {0}rel=\"{1}\" title=\"{2}\" href=\"{3}\" />";
            page.Header.Controls.Add(new LiteralControl(string.Format(tag, tp, relation, title, href)));
        }
        /// <summary>
        /// Add javascript to page
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="src">Source</param>
        /// <param name="top">If add to page header</param>
        /// <param name="defer">Defer</param>
        /// <param name="asnc">Async</param>
        /// <param name="indx">Index to insert script at</param>
        public static void AddScript(System.Web.UI.Page page, string src, bool top = true, bool defer = false, bool asnc = false, int indx = 0)
        {
            var d = defer ? " defer=\"defer\"" : "";
            var a = asnc ? " async=\"async\"" : "";
            var t = "\n<script type=\"text/javascript\" src=\"{0}\"{1}{2}></script>";
            t = string.Format(t, src, d, a);

            if (top)
            {
                page.Header.Controls.AddAt(indx, new LiteralControl(t));
            }
            else
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), src.GetHashCode().ToString(), t, false);
            }
        }
        /// <summary>
        /// Format inline script
        /// </summary>
        /// <param name="script">JavaScript code</param>
        /// <returns>Formatted script</returns>
        public static string FormatInlineScript(string script)
        {
            var sb = new StringBuilder();

            sb.Append("\n<script type=\"text/javascript\"> \n");
            sb.Append("//<![CDATA[ \n");
            sb.Append(script).Append(" \n");
            sb.Append("//]]> \n");
            sb.Append("</script> \n");

            return sb.ToString();
        }

        #region BlogBasePage helpers

        /// <summary>
        /// Adds code to the HTML head section.
        /// </summary>
        public static void AddCustomCodeToHead(System.Web.UI.Page page)
        {
            if (string.IsNullOrEmpty(BlogSettings.Instance.HtmlHeader))
                return;

            var code = string.Format(
                CultureInfo.InvariantCulture,
                "{0}<!-- Start custom code -->{0}{1}{0}<!-- End custom code -->{0}",
                Environment.NewLine,
                BlogSettings.Instance.HtmlHeader);
            var control = new LiteralControl(code);
            page.Header.Controls.Add(control);
        }

        /// <summary>
        /// Adds a JavaScript to the bottom of the page at runtime.
        /// </summary>
        /// <remarks>
        /// You must add the script tags to the BlogSettings.Instance.TrackingScript.
        /// </remarks>
        public static void AddTrackingScript(System.Web.UI.Page page)
        {
            var sb = new StringBuilder();

            if (BlogSettings.Instance.ModerationType == BlogSettings.Moderation.Disqus)
            {
                sb.Append("<script type=\"text/javascript\"> \n");
                sb.Append("//<![CDATA[ \n");
                sb.Append("(function() { ");
                sb.Append("var links = document.getElementsByTagName('a'); ");
                sb.Append("var query = '?'; ");
                sb.Append("for(var i = 0; i < links.length; i++) { ");
                sb.Append("if(links[i].href.indexOf('#disqus_thread') >= 0) { ");
                sb.Append("query += 'url' + i + '=' + encodeURIComponent(links[i].href) + '&'; ");
                sb.Append("}}");
                sb.Append("document.write('<script charset=\"utf-8\" type=\"text/javascript\" src=\"http://disqus.com/forums/");
                sb.Append(BlogSettings.Instance.DisqusWebsiteName);
                sb.Append("/get_num_replies.js' + query + '\"></' + 'script>'); ");
                sb.Append("})(); \n");
                sb.Append("//]]> \n");
                sb.Append("</script> \n");
            }

            if (!string.IsNullOrEmpty(BlogSettings.Instance.TrackingScript))
            {
                sb.Append(BlogSettings.Instance.TrackingScript);
            }

            var s = sb.ToString();
            if (!string.IsNullOrEmpty(s))
            {
                page.ClientScript.RegisterStartupScript(page.GetType(), "tracking", string.Format("\n{0}", s), false);
            }
        }

        /// <summary>
        /// Add bundles created by web.optimization
        /// </summary>
        /// <param name="page">Base page</param>
        public static void AddBundledStylesAndScripts(System.Web.UI.Page page)
        {
            var headerScripts = new List<string>();
            var globalScripts = new List<string>();
            var resourcePath = HttpHandlers.ResourceHandler.GetScriptPath(new CultureInfo(BlogSettings.Instance.Language));
            
            headerScripts.Add(resourcePath);
            headerScripts.Add(string.Format("{0}Scripts/Header/js", Utils.ApplicationRelativeWebRoot));
            
            if (Security.IsAuthenticated)
            {
                AddStyle(page, string.Format("{0}Styles/cssauth", Utils.ApplicationRelativeWebRoot));
                globalScripts.Add(string.Format("{0}Scripts/jsauth", Utils.ApplicationRelativeWebRoot));
            }
            else
            {
                AddStyle(page, string.Format("{0}Styles/css", Utils.ApplicationRelativeWebRoot));
                globalScripts.Add(string.Format("{0}Scripts/js", Utils.ApplicationRelativeWebRoot));
            }

            AddGlobalScriptsToThePage(page, headerScripts, globalScripts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public static void AddStylesAndScripts(System.Web.UI.Page page)
        {
            var headerScripts = new List<string>();
            var globalScripts = new List<string>();
            var resourcePath = HttpHandlers.ResourceHandler.GetScriptPath(new CultureInfo(BlogSettings.Instance.Language));
            List<string> files = new List<string>();

            files = GetFiles(string.Format("{0}Styles", Utils.ApplicationRelativeWebRoot));
            foreach (var f in files)
            {
                AddStyle(page, string.Format("{0}Styles/{1}", Utils.ApplicationRelativeWebRoot, f));
            }

            if (Security.IsAuthenticated)
            {
                AddStyle(page, string.Format("{0}Modules/QuickNotes/Qnotes.css", Utils.ApplicationRelativeWebRoot));
            }

            headerScripts.Add(resourcePath);

            files = GetFiles(string.Format("{0}Scripts/Header", Utils.ApplicationRelativeWebRoot));
            foreach (var f in files)
            {
                headerScripts.Add(string.Format("{0}Scripts/Header/{1}", Utils.ApplicationRelativeWebRoot, f));
            }

            files = GetFiles(string.Format("{0}Scripts", Utils.ApplicationRelativeWebRoot));
            foreach (var f in files)
            {
                globalScripts.Add(string.Format("{0}Scripts/{1}", Utils.ApplicationRelativeWebRoot, f));
            }

            if (Security.IsAuthenticated)
            {
                globalScripts.Add(string.Format("{0}admin/widget.js", Utils.ApplicationRelativeWebRoot));
                globalScripts.Add(string.Format("{0}Modules/QuickNotes/Qnotes.js", Utils.ApplicationRelativeWebRoot));
            }

            AddGlobalScriptsToThePage(page, headerScripts, globalScripts);
        }

        #endregion

        static List<string> GetFiles(string url)
        {
            List<string> files = new List<string>();
            string path = System.Web.HttpContext.Current.Server.MapPath(url);

            var folder = new DirectoryInfo(path);
            if (folder.Exists)
            {
                foreach (var file in folder.GetFiles())
                {
                    files.Add(file.Name);
                }
            }
            return files;
        }

        static int GetIndex(System.Web.UI.Page page)
        {
            // insert global scripts just before first script tag in the header
            // or after last css style tag if no script tags in the header found
            int cnt = 0;
            int idx = 0;
            string ctrlText = "";

            foreach (Control ctrl in page.Header.Controls)
            {
                cnt++;
                try
                {
                    if (ctrl.GetType() == typeof(LiteralControl))
                    {
                        LiteralControl lc = (LiteralControl)ctrl;
                        ctrlText = lc.Text.ToLower();
                    }
                    if (ctrl.GetType() == typeof(HtmlLink))
                    {
                        HtmlLink hl = (HtmlLink)ctrl;
                        ctrlText = hl.Attributes["type"].ToLower();
                    }
                    if (ctrlText.Contains("text/css"))
                        idx = cnt;

                    if (ctrlText.Contains("text/javascript"))
                    {
                        idx = cnt;
                        // offset by 1 as we need inject before
                        if (idx > 1) idx = idx - 1;
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            return idx;
        }

        static void AddGlobalScriptsToThePage(System.Web.UI.Page page, List<string> headerScripts, List<string> globalScripts)
        {
            int idx = GetIndex(page);

            globalScripts.Reverse();
            foreach (var gs in globalScripts)
            {
                AddScript(page, gs, true, true, true, idx);
            }

            headerScripts.Reverse();
            foreach (var hs in headerScripts)
            {
                AddScript(page, hs, true, false, false, idx);
            }
        }
    
    }
}