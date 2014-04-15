namespace KnockoutMvc
{
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;

    public abstract class KnockoutController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            KnockoutUtilities.ConvertData(data);
            return base.Json(data, contentType, contentEncoding);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            KnockoutUtilities.ConvertData(data);
            return base.Json(data, contentType, contentEncoding, behavior);
        }

        protected KnockoutJsonResult JsonPackage(object data)
        {
            KnockoutUtilities.ConvertData(data);
            return new KnockoutJsonResult(data, this.ModelState);
        }

        protected KnockoutJsonResult JsonPackage(object data, string message)
        {
            KnockoutUtilities.ConvertData(data);
            return new KnockoutJsonResult(data, message, this.ModelState);
        }

        protected KnockoutJsonResult JsonRedirect(string url)
        {
            return new KnockoutJsonResult(url);
        }

        #region RedirectToAction

        protected KnockoutJsonResult JsonRedirectToAction(string actionName, object routeValues)
        {
            return this.JsonRedirectToAction(actionName, null, new RouteValueDictionary(routeValues));
        }

        protected KnockoutJsonResult JsonRedirectToAction(string actionName, RouteValueDictionary routeValues)
        {
            return this.JsonRedirectToAction(actionName, null, routeValues);
        }

        protected KnockoutJsonResult JsonRedirectToAction(string actionName, string controllerName, object routeValues)
        {
            return this.JsonRedirectToAction(actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        protected virtual KnockoutJsonResult JsonRedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return new KnockoutJsonResult(this.Url.Action(actionName, controllerName, routeValues));
        }

        #endregion

        #region RedirectToRoute

        protected KnockoutJsonResult JsonRedirectToRoute(string routeName, object routeValues)
        {
            return this.JsonRedirectToRoute(routeName, new RouteValueDictionary(routeValues));
        }

        protected virtual KnockoutJsonResult JsonRedirectToRoute(string routeName, RouteValueDictionary routeValues)
        {
            return new KnockoutJsonResult(this.Url.RouteUrl(routeName, routeValues));
        }

        #endregion
    }
}
