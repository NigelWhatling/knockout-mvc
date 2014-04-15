namespace KnockoutMvc
{
    using System.Web.Mvc;

    public class KnockoutWithContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutWithContext(KnockoutContext<TModel> context, string expression)
            : base(context, expression)
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