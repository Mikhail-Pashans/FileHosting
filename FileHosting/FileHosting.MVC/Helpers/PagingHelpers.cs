using FileHosting.MVC.ViewModels;
using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace FileHosting.MVC.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PageInfo pageInfo, Func<int, string> pageUrl)
        {
            var result = new StringBuilder();            
           
            var firstATag = new TagBuilder("a") { InnerHtml = "«" };
            firstATag.AddCssClass("btn btn-default");
            var lastATag = new TagBuilder("a") { InnerHtml = "»" };
            lastATag.AddCssClass("btn btn-default");

            firstATag.MergeAttribute("href", pageUrl(pageInfo.PageNumber - 1));
            lastATag.MergeAttribute("href", pageUrl(pageInfo.PageNumber + 1));
            
            if (pageInfo.PageNumber == 1)
            {
                firstATag.MergeAttribute("href", "javascript:void(0)");
                firstATag.AddCssClass("disabled");
            }
            if (pageInfo.PageNumber == pageInfo.TotalPages)
            {
                lastATag.MergeAttribute("href", "javascript:void(0)");
                lastATag.AddCssClass("disabled");
            }

            result.Append(firstATag);

            for (var i = 1; i <= pageInfo.TotalPages; i++)
            {
                var aTag = new TagBuilder("a") { InnerHtml = i.ToString(CultureInfo.InvariantCulture) };
                aTag.MergeAttribute("href", pageUrl(i));
                
                if (i == pageInfo.PageNumber)
                {
                    aTag.AddCssClass("active");
                    aTag.AddCssClass("btn btn-primary");
                }
                aTag.AddCssClass("btn btn-default");
                
                result.Append(aTag);
            }

            result.Append(lastATag);

            var divTag = new TagBuilder("div");
            divTag.AddCssClass("btn-group");
            divTag.AddCssClass("pagination");
            divTag.InnerHtml = result.ToString();

            return MvcHtmlString.Create(divTag.ToString());
        }
    }
}