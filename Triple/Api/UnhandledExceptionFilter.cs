using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Serilog;

namespace Triple.Api
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var unauthorizedAccessException = context.Exception as UnauthorizedAccessException;
            if (unauthorizedAccessException != null)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "Access not allowed!");
                return;
            }

            var appSettingsException = context.Exception as AppSettingsException;
            if (appSettingsException != null)
            {
                Log.Error(appSettingsException, "::AppSettingsException");
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Invalid configuration, please contact the system administrator!", appSettingsException);
                return;
            }

            Log.Error(context.Exception, "Unhandled Exception");
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                "An error occured, please try again!", context.Exception);
        }
    }
}
