namespace KnockoutMvc
{
    using System.Web.Mvc;

    public class KnockoutModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = base.BindModel(controllerContext, bindingContext);
            KnockoutUtilities.ConvertData(result);
            return result;
        }
    }
}