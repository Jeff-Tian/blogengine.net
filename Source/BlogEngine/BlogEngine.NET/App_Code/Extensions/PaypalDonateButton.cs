using System;
using BlogEngine.Core;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Text;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;

/// <summary>
/// Add Paypal Donate buttons to a post or page
/// By Mike Teye http://www.levitical.org
/// </summary>
[Extension("paypaldonatebutton", "1.0.0.0", "http://www.levitical.org")]
public class paypaldonatebutton
{

    private static ExtensionSettings settings;
    private const int _controlsHeight = 20;

    private const string _userNameDefault = "Set your Company Name Ltd";
    private const string _serverUrlDefault = "https://www.sandbox.paypal.com/us/cgi-bin/webscr";
    private const string _buttonUrlDefault = "https://www.paypal.com/en_GB/i/btn/btn_donateCC_LG.gif";
    private const string _businessEmailDefault = "xxx@yyy.com";
    private const string _currencyCodeDefault = "GBP";
    private const double _taxRateDefault = 0.0;
    private const double _shippingDefault = 0.00;
        

    public paypaldonatebutton()
    {
        Post.Serving += new EventHandler<ServingEventArgs>(Post_Serving);
        BlogEngine.Core.Page.Serving += new EventHandler<ServingEventArgs>(Post_Serving);

        ExtensionSettings initialSettings = new ExtensionSettings((GetType().Name));

        initialSettings.Help = "Usage: Insert [donate:item_name] in your post to create paypal donate buttons with reference item_name. You need a paypal account. Change url to live server when ready to go live.(live url:https://www.paypal.com/cgi-bin/webscr, sandbox url:https://www.sandbox.paypal.com/us/cgi-bin/webscr)";

        initialSettings.AddParameter("userName", "Company Name");
        initialSettings.AddValue("userName", _userNameDefault.ToString());

        initialSettings.AddParameter("serverUrl", "server Url");
        initialSettings.AddValue("serverUrl", _serverUrlDefault.ToString());

        initialSettings.AddParameter("buttonUrl", "Absolute Path Button Url");
        initialSettings.AddValue("buttonUrl", _buttonUrlDefault.ToString());

        initialSettings.AddParameter("currencyCode", "Currency Code (3 chars)");
        initialSettings.AddValue("currencyCode", _currencyCodeDefault.ToString());

        initialSettings.AddParameter("taxRate", "Tax Rate in Decimal");
        initialSettings.AddValue("taxRate", _taxRateDefault.ToString());

        initialSettings.AddParameter("shipping", "Shipping Charge");
        initialSettings.AddValue("shipping", _shippingDefault.ToString());

        initialSettings.AddParameter("businessEmail", "Pay pal email address");
        initialSettings.AddValue("businessEmail", _businessEmailDefault.ToString());

        initialSettings.IsScalar = true;
        ExtensionManager.ImportSettings(initialSettings);
        settings = ExtensionManager.GetSettings(GetType().Name);
        
    }

    /// <summary>
    /// An event that handles ServingEventArgs
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Post_Serving(object sender, ServingEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Body))
        {
            // only process the posts
            if (e.Location == ServingLocation.PostList || e.Location == ServingLocation.SinglePost || e.Location == ServingLocation.SinglePage)
            {
                string regex = @"\[donate:.*]";
                MatchCollection matches = Regex.Matches(e.Body, regex);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string item_name = match.Value.Replace("[donate:", "").Replace("]", "");
                        string literal_donate = "";
                        literal_donate = GetDonateButton(item_name, double.Parse(settings.GetSingleValue("taxRate")), double.Parse(settings.GetSingleValue("shipping")), settings.GetSingleValue("username"));
                        e.Body = e.Body.Replace(match.Value, literal_donate);
                    }
                }
            }
        }
    }

    // <summary>
// Creates a link for a Donate button
// </summary>
// <param name="itemName"></param>
// <param name="tax"></param>
// <param name="shipping"></param>
// <param name="userName"></param>
// <returns></returns>
public static string GetDonateButton(string itemName, double tax, double shipping, string userName)
 {
StringBuilder url = new StringBuilder();

string serverURL = settings.GetSingleValue("serverUrl");

    url.Append(serverURL + "?cmd=_donations&currency_code=" + settings.GetSingleValue("currencyCode") + "&business=" + HttpUtility.UrlEncode(settings.GetSingleValue("businessEmail")));
url.Append("&no_shipping=" + "2");
    url.Append("&no_note=" + "1");

if (tax > 0) {
url.AppendFormat("&tax=" + tax.ToString().Replace(",", "."));
}

if (shipping > 0) {
url.AppendFormat("&shipping=" + shipping.ToString().Replace(",", "."));
}

    url.AppendFormat("&item_name={0}", HttpUtility.UrlEncode(userName));
url.AppendFormat("&item_number={0}", HttpUtility.UrlEncode(itemName));
    url.Append("&undefined_quantity=1");
url.AppendFormat("&custom={0}", HttpUtility.UrlEncode(userName));


return "<a href='" + url.ToString() + "' target=_blank><img src='" + settings.GetSingleValue("buttonUrl") + "' border='0' alt='Donate with PayPal - it's fast, free and secure!'></a>";
  }
}