#region Using

using System;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using System.Web;
using BlogEngine.Core.Web.Extensions;

#endregion

[Extension("Inserts Adsense code at the beginning or end of posts or pages", "<a href=\"http://www.cristianofino.net/post/Adsense-Injector-extension-per-BlogEngineNET.aspx\">2.0.0</a>", "<a href=\"http://www.cristianofino.net\">Cristiano Fino</a>")]
public class AdsenseInjector : BlogParser.ISubscriber
{
    static protected ExtensionSettings _settings = null;
    static protected string _noAdsense = "[noadsense]";
    static protected string _adForever = "forever";

    private static string _Html;
    /// <summary>
    /// Gets the HTML used to inserts the AdSense code.
    /// </summary>
    public static string Html
    {
        get
        {
            if (_Html == null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div style=\"{5}\">\n");
                //sb.Append("<script type=\"text/javascript\">\n<!--\ngoogle_ad_client = \"{0}\";\n");
                sb.Append("<script type=\"text/javascript\">\n\ngoogle_ad_client = \"{0}\";\n");
                sb.Append("/* {1} */\n");
                sb.Append("google_ad_slot = \"{2}\";\ngoogle_ad_width = {3};\ngoogle_ad_height = {4};\n\n");
                sb.Append("</script>\n<script type=\"text/javascript\" src=\"http://pagead2.googlesyndication.com/pagead/show_ads.js\">\n");
                sb.Append("</script>\n</div>\n");
                _Html = sb.ToString();
            }
            return _Html;
        }
    }

    public AdsenseInjector()
    {
        Post.Serving += new EventHandler<ServingEventArgs>(Serving);
        Page.Serving += new EventHandler<ServingEventArgs>(Serving);

        ExtensionSettings settings = new ExtensionSettings("AdsenseInjector");

        settings.AddParameter("ad_Author", "Author (write \"" + _adForever + "\" to insert the ad always)", 50, true);
        settings.AddParameter("ad_PubID", "Publisher ID", 50, true);
        settings.AddParameter("ad_Desc", "Description", 200);
        settings.AddParameter("ad_ID", "ID Slot", 50, true, true);
        settings.AddParameter("ad_size_w", "Size (width)", 4, true);
        settings.AddParameter("ad_size_h", "Size (height)", 4, true);
        settings.AddParameter("ad_style", "Style code", 500, true);
        settings.AddParameter("ad_position", "Position ([T]op, [C]enter, [B]ottom, [R]andom)", 1, true);
        settings.AddParameter("ad_display", "Display ([A]lways, [R]eferrer, [S]earch engine)", 1, true);
        settings.AddParameter("ad_where", "Insert in [P]ost, P[A]ges, [B]oth", 1);

        settings.Help = "Inject Adsense Code in the posts and pages. All parameters are mandatory. The code can be added at the beginning, middle or end of the post (or page)";

        ExtensionManager.ImportSettings(settings);
        _settings = ExtensionManager.GetSettings("AdsenseInjector");

    }

    /// <summary>
    /// Returns the position of the first character of the paragraph at the centre of post
    /// </summary>
    /// <param name="body">String text to parse</param>
    /// <returns></returns>
    private static Int32 BodyCenter(string body)
    {
        Regex R = new Regex(@"<p[^>]*>(.*?)<\/p>", RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                   RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
        MatchCollection par = R.Matches(body);
        if (par.Count > 1)
            return (par[par.Count / 2].Index);
        else
            return (0);
    }

    /// <summary>
    /// Serving AdSense code in post and page
    /// </summary>
    private static void Serving(object sender, ServingEventArgs e)
    {
        if (_settings.Parameters.Count > 0)
        {
            if ((e.Location == ServingLocation.SinglePage || e.Location == ServingLocation.SinglePost)
                && (!e.Body.Contains(_noAdsense)) && (!System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated))
            {
                DataTable table = _settings.GetDataTable();
                Int32 count = 0;
                // Post or page ?
                bool where = false;
                // Referrer Type
                string referrer_type = "A";
                // Referring object: verify if post or page and assign the author
                string Author = "";
                if (e.Location == ServingLocation.SinglePost)
                {
                    Post post = (Post)sender;
                    Author = post.Author.ToLower();
                }
                if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    string referrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLowerInvariant();
                    if (!referrer.Contains(Utils.AbsoluteWebRoot.Host.ToString()) && referrer != "")
                    {
                        referrer_type += "R";
                        if (!referrer.Contains("q=")) referrer_type += "S";
                    }
                }

                foreach (DataRow row in table.Rows)
                {
                    if ((((string)row["ad_author"]).ToLower()) == Author || (((string)row["ad_author"]).ToLower()) == _adForever)
                    {
                        if ((((string)row["ad_where"]).ToUpper()) == "B" || (((string)row["ad_where"]).ToUpper()) == "")
                            where = true;
                        else
                        {
                            if ((((string)row["ad_where"]).ToUpper()) == "P" && e.Location == ServingLocation.SinglePost)
                                where = true;
                            else
                            {
                                if ((((string)row["ad_where"]).ToUpper()) == "A" && e.Location == ServingLocation.SinglePage)
                                    where = true;
                                else where = false;
                            }
                        }

                        if (referrer_type.Contains(((string)row["ad_display"]).ToUpper()) && (string)row["ad_Desc"] != "" && where == true)
                        {
                            string _adScript = string.Format(Html, (string)row["ad_PubID"], (string)row["ad_Desc"],
                                                            (string)row["ad_ID"], (string)row["ad_size_w"], (string)row["ad_size_h"],
                                                            (string)row["ad_style"]);
                            string _adPosition = ((string)row["ad_position"]).ToUpper();
                            if (_adPosition == "R")
                            {
                                Random autoRand = new Random();
                                string choices = "TCB";
                                _adPosition = choices.Substring(autoRand.Next(3), 1);
                            }
                            switch (_adPosition)
                            {
                                case "T":
                                    e.Body = _adScript + e.Body;
                                    break;
                                case "C":
                                    Int32 _center = BodyCenter(e.Body);
                                    if (_center > 0)
                                        e.Body = e.Body.Substring(0, _center - 1) + _adScript + e.Body.Substring(_center);
                                    break;
                                case "B":
                                    e.Body += _adScript;
                                    break;
                                default:
                                    e.Body = _adScript + e.Body;
                                    break;
                            }
                            count += 1;
                        }
                    }
                    if (count == 3) break;
                }
            }

        }
        if (e.Location != ServingLocation.Feed)
            //e.Body = "<!-- google_ad_section_start -->\n" + e.Body + "<!-- google_ad_section_end -->\n";
            // if post or page contains noAdsense string then replace with null
            if (e.Body.Contains(_noAdsense))
                e.Body = e.Body.Replace(_noAdsense, "");
    }


    public void TagFound(BlogParser.HtmlTagArgs e)
    {
        throw new NotImplementedException();
    }

    public void DomReady(BlogParser.HtmlDomArgs e)
    {
        throw new NotImplementedException();
    }
}