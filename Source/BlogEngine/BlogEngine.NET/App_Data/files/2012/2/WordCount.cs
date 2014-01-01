#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.Extensions;
using System.Text.RegularExpressions;

#endregion

/// <summary>
///   Zusammenfassungsbeschreibung für GEBEEWordCount
/// </summary>
[Extension("Calculate word count for posts", "2.5",
	"<a href=\"http://www.zizhujy.com/blog/?tag=/word-count\">Jeff</a>")]
public class WordCount
{
	#region Fields (1) 

	private static Dictionary<Guid, int> m_WordCountList = new Dictionary<Guid, int>();

	#endregion Fields 

	#region Constructors (1) 

	static WordCount()
	{
		if(ExtensionManager.ExtensionEnabled("WordCount"))
		{
			Post.Serving += new EventHandler<ServingEventArgs>( Post_Serving);
			Post.Saved += new EventHandler<SavedEventArgs>( Post_Saved);
		}
	}

	#endregion Constructors 

	#region Methods (3) 

	// Public Methods (1) 

	public static int GetWordCount(Post post)
	{
		if(m_WordCountList.ContainsKey(post.Id)) //ist schon drin
		{
			return m_WordCountList[post.Id];
		}

		//Wörter zählen. Ergibt die selben Resultate wie bei MS Word. :-)
        // Jeff: With Chinese words, the result is not correct!
		//var words = Utils.StripHtml(post.Content).Split(' ').Where(w => w.Length > 0);
        
		//m_WordCountList.Add(post.Id, words.Count());

        //return words.Count().ToString();

        var wordCount = CalculateWordCount(Utils.StripHtml(post.Content));
        m_WordCountList.Add(post.Id, wordCount);

        return wordCount;
	}

    /// <summary>
    /// Calculate word count
    /// </summary>
    /// <returns></returns>
    private static int CalculateWordCount(string article){
        var sec = Regex.Split(article, @"\s");
        int count = 0;
        foreach (var si in sec)
        {
            int ci = Regex.Matches(si, @"[\u0000-\u00ff]+").Count;
            foreach (var c in si)
                if ((int)c > 0x00FF) ci++;

            count += ci;
        }

        return count;
    }

	// Private Methods (2) 

	private static void Post_Saved(object sender, SavedEventArgs e)
	{
		Post post = sender as Post;

		//Nach dem speichern muss neu gezählt werden. Könnte ja änderungen gegeben haben.
		m_WordCountList.Remove(post.Id);
	}

	private static void Post_Serving(object sender, ServingEventArgs e)
	{
	}

	#endregion Methods 
}