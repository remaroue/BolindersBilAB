using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Infrastructure
{
    public class ErrorHandlerMiddleWare
    {
        private RequestDelegate next;

        public ErrorHandlerMiddleWare(RequestDelegate Next)
        {
            next = Next;
        }

        public async Task Invoke(HttpContext context)
        {
            await next.Invoke(context);
            if (context.Response.StatusCode == 404)
            {
                context.Response.Redirect("/404");
            }
        }

    }
}
