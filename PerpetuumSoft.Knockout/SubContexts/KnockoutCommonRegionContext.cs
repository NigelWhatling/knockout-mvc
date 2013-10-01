﻿using System.IO;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
  public abstract class KnockoutCommonRegionContext<TModel> : KnockoutRegionContext<TModel>   
  {
    public string Expression { get; set; }    

    public KnockoutCommonRegionContext(ViewContext viewContext, string expression)
      :base(viewContext)
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