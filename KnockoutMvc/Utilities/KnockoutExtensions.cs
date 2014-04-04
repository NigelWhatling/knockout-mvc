namespace KnockoutMvc
{
    using System.Web.Mvc;

    public static class KnockoutExtensions
    {
        public static KnockoutContext<TModel> CreateKnockoutContext<TModel>(this HtmlHelper<TModel> helper, bool useAntiForgeryToken = true)
        {
            return new KnockoutContext<TModel>(helper.ViewContext) { UseAntiForgeryToken = useAntiForgeryToken };
        }
    }
}
