using System;
using System.Collections.Generic;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using MarkdownDeep;
using BlogEngine.Core.Web.Extensions;

[Extension(@"Enables you writing posts through mark down markup.", "0.1", @"<a href=""http://zizhujy.com/Home/About"" target=""_blank"" title=""Jeff Tian"">Jeff Tian</a>")]
public class MarkdownDeepFormatter
{
	static protected Dictionary<Guid, ExtensionSettings> _options = new Dictionary<Guid, ExtensionSettings>();

	public MarkdownDeepFormatter()
	{
		Post.Serving += Post_Serving;

		// load the options at startup by calling the getter
		var x = Options;
	}

	void Post_Serving(object sender, ServingEventArgs e)
	{
	    var md = new Markdown();

		e.Body = md.Transform(e.Body);
	}

	private static readonly object lockObj = new object();

	private static ExtensionSettings Options
	{
		get
		{
			var blogId = Blog.CurrentInstance.Id;
			ExtensionSettings options = null;
			_options.TryGetValue(blogId, out options);

			if (options == null)
			{
				lock (lockObj)
				{
					_options.TryGetValue(blogId, out options);

					if (options == null)
					{
						// options
						options = new ExtensionSettings("MarkdownDeepExtension")
						{
						    IsScalar = true
						};

					    options.Parameters = new List<ExtensionParameter>();
                        
						_options[blogId] = ExtensionManager.InitSettings("MarkdownDeepExtension", options);
					}
				}
			}

			return options;
		}
	}
}
