namespace KnockoutMvc
{
    using System.Web.Mvc;

    public class KnockoutForeachContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        private bool useVirtualElement;

        public KnockoutForeachContext(KnockoutContext<TModel> context, string expression, bool useVirtualElement = true)
            : base(context, expression)
        {
            this.useVirtualElement = useVirtualElement;
        }

        public override void WriteStart(System.IO.TextWriter writer)
        {
            if (useVirtualElement)
            {
                base.WriteStart(writer);
            }
        }

        protected override void WriteEnd(System.IO.TextWriter writer)
        {
            if (useVirtualElement)
            {
                base.WriteEnd(writer);
            }
        }

        protected override string Keyword
        {
            get
            {
                return "foreach";
            }
        }
    }
}
