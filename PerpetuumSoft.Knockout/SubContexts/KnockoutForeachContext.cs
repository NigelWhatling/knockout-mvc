using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
  public class KnockoutForeachContext<TModel> : KnockoutCommonRegionContext<TModel>
  {
      private bool useVirtualElement;

    public KnockoutForeachContext(ViewContext viewContext, string expression, bool useVirtualElement = true) : base(viewContext, expression)
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
