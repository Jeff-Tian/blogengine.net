using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;
using BlogEngine.Core;
using System.Text.RegularExpressions;

/// <summary>
///PostViewed 的摘要说明
/// </summary>
/// 
[Extension("Counts the number of views for a post", "1.0", "<a href=\"http://www.zizhujy.com/blog\">Jeff</a>")]
public class PostViewed
{
    static protected ExtensionSettings _settings = null;

	public PostViewed()
	{
        Post.Serving += new EventHandler<ServingEventArgs>(OnPostServing);

        InitSettings();
	}

    private void OnPostServing(object sender, ServingEventArgs e)
    {
        Post post = (Post)sender;

        if (bool.Parse(_settings.GetSingleValue("ExcludeAuthenticatedViews")) && HttpContext.Current.Request.IsAuthenticated)
            return;

        string pattern = _settings.GetSingleValue("ExcludeViewsFromIPs");
        string ip = HttpContext.Current.Request.UserHostAddress;
        bool matchedIp = (!string.IsNullOrEmpty(pattern) && Regex.IsMatch(ip, pattern));

        // Do not count view of authenticated users and users who have IPs match the pattern
        if (matchedIp)
            return;

        if ( e.Location.Equals( ServingLocation.SinglePost) )
        {
            post.View();
        }
    }

    private void InitSettings()
    {
        // Create extensionSettings with Type Name
        ExtensionSettings settings = new ExtensionSettings(this.GetType().Name);

        // Define settings as Scalar
        settings.IsScalar = true;
        settings.AddParameter("ExcludeAuthenticatedViews", "Exclude Authenticated Views", 5, false, false, ParameterType.Boolean);
        settings.AddParameter("ExcludeViewsFromIPs", "<a href=\"https://www.google.com/support/googleanalytics/bin/answer.py?answer=55572\" target=\"_blank\">Execlude Views From IPs</a>", 255, false, false, ParameterType.String);

        // Add default values
        settings.AddValues(new string[] { "True", "" });

        // Build Help text
        settings.Help = "<p>Check the <strong>Exclude Authenticated Views</strong> checkbox if you wish " +
           "<strong>exclude the authenticated users</strong> from accumulating post view count.<br/>" +
            "Set <strong>Execlude Views From IPs(regex)</strong> field if you wish to execlude range of IP addresses " +
            "from accumulating post view count. This is Regular Expression field.</p>" +
           "You can use <a href=\"https://www.google.com/support/googleanalytics/bin/answer.py?answer=55572\" target=\"_blank\">this tool</a> " +
            "to generate your range of IPs ";

        // Import Settings into BlogEngine
        ExtensionManager.ImportSettings(settings);

        // Retrieve settings from BlogEngine into local static field to be used later 
        _settings = ExtensionManager.GetSettings(this.GetType().Name);
    }
}