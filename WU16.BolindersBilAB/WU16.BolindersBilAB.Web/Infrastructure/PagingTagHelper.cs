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
using System.Web;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Infrastructure
{
    [HtmlTargetElement("div", Attributes ="page-model, page-action, page-query")]
    public class PagingTagHelper : TagHelper
    {
        private IUrlHelperFactory _helper;
        public PagingTagHelper(IUrlHelperFactory Helper)
        {
            _helper = Helper;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public QueryString PageQuery { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(PageModel.CurrentPage * PageModel.ItemsPerPage < PageModel.TotalItems)
            {              
                IUrlHelper urlHelper = _helper.GetUrlHelper(ViewContext);
                var newQuery = "";
                var parameters = PageQuery.Value.Split("&").ToList();

                parameters.Remove("page=" + (PageModel.CurrentPage).ToString());


                if (PageQuery.Value.Contains("?search"))
                {
                    parameters.Add("page=" + (PageModel.CurrentPage + 1).ToString());
                    newQuery = string.Join("&", parameters);
                }
                else
                {
                    newQuery = "?page=" + (PageModel.CurrentPage + 1).ToString();
                }

                var tag = new TagBuilder("a");
                tag.AddCssClass("btn btn-primary mx-center");
                tag.Attributes["id"] = "showMore";

                tag.Attributes["href"] = ViewContext.HttpContext.Request.Path + newQuery;
                tag.InnerHtml.Append("Visa fler");

                output.Content.AppendHtml(tag);
            }
        }
    }
}
