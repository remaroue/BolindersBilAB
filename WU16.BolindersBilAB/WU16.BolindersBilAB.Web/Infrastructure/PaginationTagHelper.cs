using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Infrastructure
{
    [HtmlTargetElement("pagination", Attributes = "page-model, page-action, page-query")]
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory _helper;
        public PaginationTagHelper(IUrlHelperFactory Helper)
        {
            _helper = Helper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public QueryString PageQuery { get; set; }
        public string PageAction { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            IUrlHelper urlHelper = _helper.GetUrlHelper(ViewContext);
            var newQuery = "";
            var parameters = PageQuery.Value.Split("&").ToList();
            if (PageQuery.Value.Contains("?search"))
            {
                parameters.Add("page=");
                newQuery = string.Join("&", parameters);
            }
            else
            {
                newQuery = "?page=";
            }

            if ((PageModel.CurrentPage - 1) > 0)
            {
                var prevButton = new TagBuilder("a");
                prevButton.AddCssClass("btn btn-primary");
                prevButton.Attributes["href"] = ViewContext.HttpContext.Request.Path + newQuery + (PageModel.CurrentPage - 1);
                prevButton.InnerHtml.AppendHtml("<span class=\"fa fa-arrow-left\"></span>");
                output.Content.AppendHtml(prevButton);
            }

            if(PageModel.TotalItems / PageModel.ItemsPerPage != 0)
            {
                var middle = new TagBuilder("span");
                middle.AddCssClass("mx-auto btn");
                middle.InnerHtml.AppendHtml($"<span>{PageModel.CurrentPage} / {PageModel.TotalItems / PageModel.ItemsPerPage}</span>");
                output.Content.AppendHtml(middle);
            }

            if (PageModel.CurrentPage < (PageModel.TotalItems / PageModel.ItemsPerPage))
            {
                var nextButton = new TagBuilder("a");
                nextButton.AddCssClass("btn btn-primary");
                nextButton.Attributes["href"] = ViewContext.HttpContext.Request.Path + newQuery + (PageModel.CurrentPage + 1);
                nextButton.InnerHtml.AppendHtml("<span class=\"fa fa-arrow-right\"></span>");
                output.Content.AppendHtml(nextButton);
            }
        }
    }
}
