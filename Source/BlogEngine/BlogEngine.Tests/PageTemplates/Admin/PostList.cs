using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class PostList : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Posts/Posts.aspx"; }
        }

        public bool DeletePostByTitle(string title, IE ie)
        {
            ie.GoTo(Url);
            ie.WaitForComplete();

            var tblPosts = ie.Table("Posts");

            if (tblPosts != null)
            {
                foreach (var row in tblPosts.TableRows)
                {
                    if (!string.IsNullOrEmpty(row.Id) && row.InnerHtml.Contains(title))
                    {
                        ie.Link("a-" + row.Id).Click();
                        return true;
                    }
                } 
            }
            return false;
        }

    }
}
