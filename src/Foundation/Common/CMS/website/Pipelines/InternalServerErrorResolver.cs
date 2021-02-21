using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.MvcEvents.Exception;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ENBDGroup.Foundation.Common.CMS.Pipelines
{
    public class InternalServerErrorResolver : ExceptionProcessor
    {
        public override void Process(ExceptionArgs args)
        {
            var customErrorsSection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            var context = args.ExceptionContext;
            var httpContext = context.HttpContext;
            var exception = context.Exception;

            Log.Info("Error - ServerError:", this);
            if (customErrorsSection.Mode != CustomErrorsMode.Off)
            {
                if (context.ExceptionHandled || httpContext == null || exception == null)
                {
                    Log.Info("Error - ServerError httpContext is null", this);
                    return;
                }
                // Create a report with exception details.
                string exceptionInfo = this.GetExceptionInfo(httpContext, exception);
                // Store the report in a session variable so we can access it from the custom error page.
                Log.Error(string.Format("There was an error in {0} : {1}", Sitecore.Context.Site.Name, exceptionInfo), this);
                // Return a 500 status code and execute the custom error page.
                httpContext.Server.ClearError();
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //httpContext.Response.Redirect(Sitecore.Context.Site.ErrorPage(), false);
                httpContext.Server.Execute(Sitecore.Context.Site.ErrorPage(), false);
            }
        }

        private string GetExceptionInfo(HttpContextBase httpContext, Exception exception)
        {
            // Generate an error report.
            var errorInfo = new StringBuilder();
            errorInfo.AppendLine(string.Concat("URL: ", httpContext.Request.Url));
            /* Snipped additional lines of report generation */
            errorInfo.AppendLine(string.Concat("Source: ", exception.Source));
            errorInfo.AppendLine(string.Concat("Message: ", exception.Message));
            errorInfo.AppendLine(string.Concat("Stacktrace: ", exception.StackTrace));

            return errorInfo.ToString();
        }
        private string GetSafeLanguage()
        {
            try
            {
                return Sitecore.Context.Language.CultureInfo.TwoLetterISOLanguageName;
            }
            catch(Exception ex)
            {
                Log.Error("Error - ServerError", ex, this);
            }
            return string.Empty;
        }
    }

    public static class SiteExtension
    {
        /// <summary>  
        /// Retrun the site unique ID  
        /// </summary>  
        /// <returns></returns>  
        public static string ErrorPage(this SiteContext site)
        {
            try
            {
                string errorPage = site.Properties["errorPage"];
                Log.Info("Info - ServerErrorPath: " + errorPage, typeof(string));

                if (!String.IsNullOrEmpty(errorPage))
                    return string.Format(errorPage);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                Log.Error("Error - ServerError", ex, typeof(string));
                return string.Empty;
            }
        }
    }
}