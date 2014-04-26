<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.PostViewBase" %>

<div class="post xfolkentry" id="post<%=Index %>">
    <h1><a href="<%=Post.RelativeLink %>" class="taggedlink"><%=Server.HtmlEncode(Post.Title) %></a></h1>
    <span class="author"><%= Resources.labels.by %> <a href="<%=VirtualPathUtility.ToAbsolute("~/") + "author/" + BlogEngine.Core.Utils.RemoveIllegalCharacters(Post.Author) %>.aspx"><%=Post.AuthorProfile != null ? Post.AuthorProfile.DisplayName : Post.Author %></a></span>
    <span class="wordCount">&nbsp;|&nbsp;<%= WordCount.GetWordCount(Post) %> <%=Resources.labels.words %></span>
    <span class="pubDate"><%=Post.DateCreated.ToString("yyyy-MM-dd HH:mm") %></span>

    <div class="clear-float"></div>
    <div class="text">
        <asp:PlaceHolder ID="BodyContent" runat="server" />
    </div>
    <div class="bottom">
        <div class="clearFloat"></div>
        <%=Rating %>
        <p class="tags">Tags: <%=TagLinks(", ") %></p>
        <p class="categories"><%=CategoryLinks(" | ") %></p>
    </div>

    <div class="footer">
        <div class="bookmarks">
            <a rel="nofollow" title="Index <%=Index %>" target="_blank" href="http://www.dotnetkicks.com/submit?url=<%=Server.UrlEncode(Post.AbsoluteLink.ToString()) %>&amp;title=<%=Server.UrlEncode(Post.Title) %>">Submit to DotNetKicks...</a>
        </div>

        <%=AdminLinks %>

        <% if (BlogEngine.Core.BlogSettings.Instance.ModerationType == BlogEngine.Core.BlogSettings.Moderation.Disqus)
           { %>
        <a rel="nofollow" href="<%=Post.PermaLink %>#disqus_thread"><%=Resources.labels.comments %></a>
        <%}
           else
           { %>
        <a rel="bookmark" href="<%=Post.PermaLink %>" title="<%=Server.HtmlEncode(Post.Title) %>">Permalink</a> |
   
        <a rel="nofollow" href="<%=Post.RelativeLink %>#comment"><%=Resources.labels.comments %> (<%=Post.ApprovedComments.Count %>)</a>
        <%} %>
     | <%= Resources.labels.view %> (<%=Post.Views %>)
   
    </div>
</div>
