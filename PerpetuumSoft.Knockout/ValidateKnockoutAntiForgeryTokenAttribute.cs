namespace PerpetuumSoft.Knockout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateKnockoutAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public const string RequestVerificationHeaderName = "X-Request-Verification-Token";

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current == null)
            {
                throw new ArgumentException("HttpContext unavailable.");
            }

            HttpContextWrapper context = new HttpContextWrapper(HttpContext.Current);
            if (context.Request.HttpMethod == WebRequestMethods.Http.Post)
            {
                if (context.Request.IsAjaxRequest())
                {
                    HttpCookie cookie = context.Request.Cookies[AntiForgeryConfig.CookieName];
                    if (cookie == null)
                    {
                        throw new HttpAntiForgeryException("Request Verification Token cookie missing.");
                    }

                    string cookieToken = cookie.Value;
                    string headerToken = context.Request.Headers[RequestVerificationHeaderName];
                    if (headerToken == null)
                    {
                        throw new HttpAntiForgeryException("Request Verification Token header missing.");
                    }

                    AntiForgery.Validate(cookieToken, headerToken);
                }
                else
                {
                    new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
                }
            }
        }
    }
}
