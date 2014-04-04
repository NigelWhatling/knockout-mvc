namespace KnockoutMvc
{
    using System.Web.Mvc;

    public class KnockoutWithContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutWithContext(ViewContext viewContext, string expression)
            : base(viewContext, expression)
        {
        }

        protected override string Keyword
        {
            get
            {
                return "with";
            }
        }
    }
}