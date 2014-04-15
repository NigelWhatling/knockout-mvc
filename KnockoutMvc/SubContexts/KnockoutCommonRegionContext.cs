namespace KnockoutMvc
{
    using System.IO;
    using System.Web.Mvc;

    public abstract class KnockoutCommonRegionContext<TModel> : KnockoutRegionContext<TModel>
    {
        public string Expression { get; set; }

        public KnockoutCommonRegionContext(KnockoutContext<TModel> context, string expression)
            : base(context)
        {
            Expression = expression;
        }

        public override void WriteStart(TextWriter writer)
        {
            writer.WriteLine(string.Format(@"<!-- ko {0}: {1} -->", Keyword, Expression));
        }

        protected override void WriteEnd(TextWriter writer)
        {
            writer.WriteLine(@"<!-- /ko -->");
        }

        protected abstract string Keyword { get; }
    }
}