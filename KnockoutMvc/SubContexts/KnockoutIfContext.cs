namespace KnockoutMvc
{
    using System.Web.Mvc;

    public class KnockoutIfContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutIfContext(KnockoutContext<TModel> context, string expression)
            : base(context, expression)
        {
        }

        protected override string Keyword
        {
            get
            {
                return "if";
            }
        }
    }
}