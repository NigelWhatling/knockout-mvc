namespace KnockoutMvc
{
    using System.Text;
    using System.Web.Mvc;

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
    }
}
