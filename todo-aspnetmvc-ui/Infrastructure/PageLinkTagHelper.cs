using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo_aspnetmvc_ui.Models.ViewModels;

namespace todo_aspnetmvc_ui.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("pagenav", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            _urlHelperFactory = helperFactory ?? throw new ArgumentNullException(nameof(helperFactory));
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            TagBuilder result = new TagBuilder("div");
            TagBuilder ulTag = new TagBuilder("ul");

            ulTag.AddCssClass("pagination pagination-sm");

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder liTag = new TagBuilder("li");
                TagBuilder innerTag;

                PageUrlValues["todoListPage"] = i;

                if (PageClassesEnabled)
                {
                    if(i == PageModel.CurrentPage)
                    {
                        innerTag = new TagBuilder("span");
                        innerTag.AddCssClass(PageClass);
                        liTag.AddCssClass(PageClassSelected);
                        liTag.Attributes["aria-current"] = "page";                                                
                    }
                    else
                    {
                        innerTag = new TagBuilder("a");
                        innerTag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        innerTag.AddCssClass(PageClass);
                        liTag.AddCssClass(PageClassNormal);
                    }
                }
                else
                {
                    innerTag = new TagBuilder("a");
                    innerTag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                }

                innerTag.InnerHtml.Append(i.ToString());
                liTag.InnerHtml.AppendHtml(innerTag);
                ulTag.InnerHtml.AppendHtml(liTag);
            }

            result.InnerHtml.AppendHtml(ulTag);
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
