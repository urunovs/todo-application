using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace todo_aspnetmvc_ui.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("dlg")]
    public class DialogViewTagHelper : TagHelper
    {
        [HtmlAttributeName("id")]
        public string ModalId { get; set; }

        [HtmlAttributeName("title")]
        public string Title { get; set; }

        [HtmlAttributeName("label")]
        public string ModalLabel { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var parentDivTag = new TagBuilder("div");
            parentDivTag.Attributes["id"] = ModalId;
            parentDivTag.Attributes["tabindex"] = "-1";
            parentDivTag.Attributes["aria-labelledby"] = ModalLabel;
            parentDivTag.Attributes["aria-hidden"] = "true";
            parentDivTag.AddCssClass("modal fade");

            var dialogDivTag = new TagBuilder("div");
            dialogDivTag.AddCssClass("modal-dialog");

            var contentDivTag = new TagBuilder("div");
            contentDivTag.AddCssClass("modal-content");

            var headerDivTag = new TagBuilder("div");
            headerDivTag.AddCssClass("modal-header");

            var headerTag = new TagBuilder("h1");
            headerTag.AddCssClass("modal-title fs-5");
            headerTag.InnerHtml.Append(Title);

            var upperCloseBtnTag = new TagBuilder("button");
            upperCloseBtnTag.Attributes["type"] = "button";
            upperCloseBtnTag.Attributes["data-bs-dismiss"] = "modal";
            upperCloseBtnTag.Attributes["aria-label"] = "Close";
            upperCloseBtnTag.AddCssClass("btn-close");

            headerDivTag.InnerHtml.AppendHtml(headerTag);
            headerDivTag.InnerHtml.AppendHtml(upperCloseBtnTag);

            contentDivTag.InnerHtml.AppendHtml(headerDivTag);

            var bodyDivTag = new TagBuilder("div");
            bodyDivTag.AddCssClass("modal-body");

            var content = await output.GetChildContentAsync();
            bodyDivTag.InnerHtml.AppendHtml(content.GetContent());

            contentDivTag.InnerHtml.AppendHtml(bodyDivTag);

            var footerDivTag = new TagBuilder("div");
            footerDivTag.AddCssClass("modal-footer");

            var innerCloseBtnTag = new TagBuilder("button");
            innerCloseBtnTag.Attributes["type"] = "button";
            innerCloseBtnTag.Attributes["data-bs-dismiss"] = "modal";
            innerCloseBtnTag.AddCssClass("btn btn-secondary");
            innerCloseBtnTag.InnerHtml.Append("Close");

            var submitInputTag = new TagBuilder("input");
            submitInputTag.Attributes["type"] = "submit";
            submitInputTag.Attributes["value"] = "Apply";
            submitInputTag.AddCssClass("btn btn-primary");

            footerDivTag.InnerHtml.AppendHtml(innerCloseBtnTag);
            footerDivTag.InnerHtml.AppendHtml(submitInputTag);

            contentDivTag.InnerHtml.AppendHtml(footerDivTag);
            dialogDivTag.InnerHtml.AppendHtml(contentDivTag);
            parentDivTag.InnerHtml.AppendHtml(dialogDivTag);

            output.Content.AppendHtml(parentDivTag);
        }
    }
}
