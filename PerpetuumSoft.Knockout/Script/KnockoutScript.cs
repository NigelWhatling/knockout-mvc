using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
  public class KnockoutScript<TModel> : KnockoutSubContext<TModel>
  {
    private readonly ViewContext viewContext;

    public KnockoutScript(ViewContext viewContext, KnockoutContext<TModel> context, string[] instancesNames = null, Dictionary<string, string> aliases = null)
      : base(context, instancesNames, aliases)
    {
      this.viewContext = viewContext;
    }

    public KnockoutScriptItem GetObservable(string name)
    {
        return new KnockoutScriptItem(this.Context.ViewModelName + "." + name + "()");
    }

    public KnockoutScriptItem GetObservable(Expression<Func<TModel, object>> prop)
    {
        string name = KnockoutExpressionConverter.Convert(prop);
        return new KnockoutScriptItem(this.Context.ViewModelName + "." + name + "()");
    }

    public KnockoutScriptItem SetObservable(string name, string value)
    {
        return new KnockoutScriptItem(this.Context.ViewModelName + "." + name + "(" + value + ")");
    }

    public KnockoutScriptItem SetObservable(Expression<Func<TModel, object>> prop, string value)
    {
        string name = KnockoutExpressionConverter.Convert(prop);
        return new KnockoutScriptItem(this.Context.ViewModelName + "." + name + "(" + value + ")");
    }
  }
}