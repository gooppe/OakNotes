using System.Web.Http.Filters;
using OakNotes.Logger;
using System;
using System.Net.Http;
using System.Net;

namespace OakNotes.Api.Filters
{
    public class RepositoryExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ArgumentException)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
            Log.Intance.Error(context.Exception);
        }
    }
}