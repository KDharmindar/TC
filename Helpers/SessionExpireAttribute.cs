using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace tutioncloud.Helpers
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            // If the browser session or authentication session has expired...
            if (session.IsNewSession || session["UserID"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, return result as a simple string, 
                    // and inform calling JavaScript code that a user should be redirected.
                    JsonResult result = new JsonResult()
                    {
                        Data = "SessionTimeout",
                        ContentType = "text/html"
                    };
                    filterContext.Result = result;
                }
                else
                {
                    // For round-trip requests,
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "Controller", "User" },
                            { "Action", "Login" }
                        });
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}