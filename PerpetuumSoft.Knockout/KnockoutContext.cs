using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace PerpetuumSoft.Knockout
{
  public interface IKnockoutContext
  {
    string GetInstanceName();
    string GetIndex();
  }

  public class KnockoutContext<TModel> : IKnockoutContext
  {
    public readonly string ViewModelName = "viewModel";

    private TModel model;

    public TModel Model
    {
      get
      {
        return model;
      }
    }

    protected List<IKnockoutContext> ContextStack { get; set; }

    public KnockoutContext(ViewContext viewContext)
    {
      this.viewContext = viewContext;
      ContextStack = new List<IKnockoutContext>();
    }

    public KnockoutContext(ViewContext viewContext, string modelName) : this(viewContext)
    {
        this.ViewModelName = modelName;
    }

    public KnockoutContext<TModel2> CreateContext<TModel2>(string modelName)
    {
        return new KnockoutContext<TModel2>(this.viewContext, modelName);
    }

    private readonly ViewContext viewContext;

    private bool isInitialized;

    private string GetInitializeData(TModel model, string searchScope, bool needBinding, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Error)
    {
      if (isInitialized)
        return String.Empty;
      isInitialized = true;
      KnockoutUtilities.ConvertData(model);
      this.model = model;

      var sb = new StringBuilder();

      var json = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = referenceLoopHandling });

      sb.AppendLine(@"<script type=""text/javascript""> ");
      sb.AppendLine(string.Format("var {0}Js = {1};", ViewModelName, json));
      var mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
      if (mappingData == "{}")
      {
        sb.AppendLine(string.Format("var {0} = ko.mapping.fromJS({0}Js); ", ViewModelName));
      }
      else
      {
        sb.AppendLine(string.Format("var {0}MappingData = {1};", ViewModelName, mappingData));
        sb.AppendLine(string.Format("var {0} = ko.mapping.fromJS({0}Js, {0}MappingData); ", ViewModelName));
      }
      sb.Append(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
      if (needBinding)
      {
          if (searchScope == null)
          {
              sb.AppendLine(string.Format("ko.applyBindings({0});", ViewModelName));
          }
          else
          {
              sb.AppendLine(string.Format(@"ko.applyBindings({0}, $(""{1}"").get(0));", ViewModelName, searchScope));
          }
      }
      sb.AppendLine(@"</script>");
      return sb.ToString();
    }

    public HtmlString Initialize(TModel model)
    {
        return this.Initialize(model, null);
    }

    public HtmlString Initialize(TModel model, string searchScope)
    {
      return new HtmlString(GetInitializeData(model, searchScope, false));
    }

    public HtmlString Apply(TModel model, string SearchScope = null, ReferenceLoopHandling ReferenceLoopHandling = ReferenceLoopHandling.Error)
    {
      if (isInitialized)
      {
        var sb = new StringBuilder();
        sb.AppendLine(@"<script type=""text/javascript"">");
        if (SearchScope == null)
        {
            sb.AppendLine(string.Format("ko.applyBindings({0});", ViewModelName));
        }
        else
        {
            sb.AppendLine(string.Format(@"ko.applyBindings({0}, $(""{1}"").get(0));", ViewModelName, SearchScope));
        }

        sb.AppendLine(@"</script>");
        return new HtmlString(sb.ToString());
      }
      return new HtmlString(GetInitializeData(model, SearchScope, true, ReferenceLoopHandling));
    }

    public HtmlString LazyApply(TModel model, string actionName, string controllerName)
    {
        return this.LazyApply(model, actionName, controllerName, null);
    }

    public HtmlString LazyApply(TModel model, string actionName, string controllerName, string searchScope)
    {
      var sb = new StringBuilder();

      sb.AppendLine(@"<script type=""text/javascript""> ");
      sb.Append("$(document).ready(function() {");

      sb.AppendLine(string.Format("$.ajax({{ url: '{0}', type: 'POST', success: function (data) {{", Url().Action(actionName, controllerName)));

      string mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
      if (mappingData == "{}")
        sb.AppendLine(string.Format("var {0} = ko.mapping.fromJS(data); ", ViewModelName));
      else
      {
        sb.AppendLine(string.Format("var {0}MappingData = {1};", ViewModelName, mappingData));
        sb.AppendLine(string.Format("var {0} = ko.mapping.fromJS(data, {0}MappingData); ", ViewModelName));
      }
      sb.Append(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
      if (searchScope == null)
      {
          sb.AppendLine(string.Format("ko.applyBindings({0});", ViewModelName));
      }
      else
      {
          sb.AppendLine(string.Format(@"ko.applyBindings({0}, $(""{1}"").get(0));", ViewModelName, searchScope));
      }

      sb.AppendLine("}, error: function (error) { alert('There was an error posting the data to the server: ' + error.responseText); } });");

      sb.Append("});");
      sb.AppendLine(@"</script>");

      return new HtmlString(sb.ToString());
    }

    private int ActiveSubcontextCount
    {
      get
      {
        return ContextStack.Count - 1 - ContextStack.IndexOf(this);
      }
    }

    public KnockoutForeachContext<TItem> Foreach<TItem>(Expression<Func<TModel, IList<TItem>>> binding)
    {
      var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
      var regionContext = new KnockoutForeachContext<TItem>(viewContext, expression);
      regionContext.WriteStart(viewContext.Writer);
      regionContext.ContextStack = ContextStack;
      ContextStack.Add(regionContext);
      return regionContext;
    }

    public KnockoutWithContext<TItem> With<TItem>(Expression<Func<TModel, TItem>> binding)
    {
      var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
      var regionContext = new KnockoutWithContext<TItem>(viewContext, expression);
      regionContext.WriteStart(viewContext.Writer);
      regionContext.ContextStack = ContextStack;
      ContextStack.Add(regionContext);
      return regionContext;
    }

    public KnockoutIfContext<TModel> If(Expression<Func<TModel, bool>> binding)
    {
      var regionContext = new KnockoutIfContext<TModel>(viewContext, KnockoutExpressionConverter.Convert(binding));
      regionContext.InStack = false;
      regionContext.WriteStart(viewContext.Writer);
      return regionContext;
    }

    public string GetInstanceName()
    {
      switch (ActiveSubcontextCount)
      {
        case 0:
          return "";
        case 1:
          return "$parent";
        default:
          return "$parents[" + (ActiveSubcontextCount - 1) + "]";
      }
    }

    private string GetContextPrefix()
    {
      var sb = new StringBuilder();
      int count = ActiveSubcontextCount;
      for (int i = 0; i < count; i++)
        sb.Append("$parentContext.");
      return sb.ToString();
    }

    public string GetIndex()
    {
      return GetContextPrefix() + "$index()";
    }

    public virtual KnockoutExpressionData CreateData()
    {
      return new KnockoutExpressionData { InstanceNames = new[] { GetInstanceName() } };
    }

    public virtual KnockoutBinding<TModel> Bind
    {
      get
      {
        return new KnockoutBinding<TModel>(this, CreateData().InstanceNames, CreateData().Aliases);
      }
    }

    public virtual KnockoutHtml<TModel> Html
    {
      get
      {
        return new KnockoutHtml<TModel>(viewContext, this, CreateData().InstanceNames, CreateData().Aliases);
      }
    }

    public virtual KnockoutScript<TModel> Script
    {
        get
        {
            return new KnockoutScript<TModel>(viewContext, this, CreateData().InstanceNames, CreateData().Aliases);
        }
    }

    public MvcHtmlString ServerAction(string actionName, string controllerName, object routeValues = null,
        bool useAntiForgeryToken = false, bool noModel = false, string bindingOut = null, KnockoutExecuteEvents events = null)
    {
        RouteValueDictionary newRoutes = new RouteValueDictionary(routeValues);
        Dictionary<string, KnockoutScriptItem> tokens = new Dictionary<string, KnockoutScriptItem>();

        if (routeValues != null)
        {
            int i = 0;
            foreach (PropertyInfo prop in routeValues.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.PropertyType.IsAssignableFrom(typeof(KnockoutScriptItem)))
                {
                    string token = "$$$" + (i++) + "$$$";
                    tokens.Add(token, (KnockoutScriptItem)prop.GetValue(routeValues, null));
                    newRoutes[prop.Name] = token;
                }
            }
        }

        var url = Url().Action(actionName, controllerName, newRoutes) ?? "/";
      url = url.Replace("%28", "(");
      url = url.Replace("%29", ")");
      url = url.Replace("%24", "$");
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("executeOnServer({0}, '{1}'", noModel ? "null" : ViewModelName, url);
      if (useAntiForgeryToken)
      {
          sb.Append(", '");
          sb.Append(this.GetAntiForgeryToken());
          sb.Append("'");
      }

      if (!String.IsNullOrWhiteSpace(bindingOut))
      {
          sb.Append(", ");
          sb.Append(bindingOut);
      }

      if (events != null)
      {
          bool isFirst = true;
          sb.Append(", {");
          if (events.BeforeSend != null)
          {
              sb.Append("beforeSend: ");
              sb.Append(events.BeforeSend);
              isFirst = false;
          }

          if (events.Complete != null)
          {
              sb.Append(isFirst ? " " : " ,");
              sb.Append("complete: ");
              sb.Append(events.Complete);
              isFirst = false;
          }
          if (events.Error != null)
          {
              sb.Append(isFirst ? " " : " ,");
              sb.Append("error: ");
              sb.Append(events.Error);
              isFirst = false;
          }
          if (events.Success != null)
          {
              sb.Append(isFirst ? " " : " ,");
              sb.Append("success: ");
              sb.Append(events.Success);
              isFirst = false;
          }

          sb.Append("}");
      }

      sb.Append(")");
      string exec = sb.ToString();
      int startIndex = 0;
      const string parentPrefix = "$parentContext.";
      while (exec.Substring(startIndex).Contains("$index()"))
      {
        string pattern = "$index()";
        string nextPattern = parentPrefix + pattern;
        int index = startIndex + exec.Substring(startIndex).IndexOf(pattern);
        while (index - parentPrefix.Length >= startIndex &&
               exec.Substring(index - parentPrefix.Length, nextPattern.Length) == nextPattern)
        {
          index -= parentPrefix.Length;
          pattern = nextPattern;
          nextPattern = parentPrefix + pattern;
        }
        exec = exec.Substring(0, index) + "'+" + pattern + "+'" + exec.Substring(index + pattern.Length);
        startIndex = index + pattern.Length;
      }
      
        foreach (string token in tokens.Keys)
        {
            exec = exec.Replace(token, "'+" + tokens[token].ToHtmlString() + "+'");
        }

      return new MvcHtmlString(exec);
    }

    protected UrlHelper Url()
    {
      return new UrlHelper(viewContext.RequestContext);
    }

    protected string GetAntiForgeryToken()
    {
        string pattern = "value=\"";
        string tokenHtml = System.Web.Helpers.AntiForgery.GetHtml().ToHtmlString();
        int start = tokenHtml.IndexOf(pattern);
        tokenHtml = tokenHtml.Substring(start + pattern.Length);
        return tokenHtml.Substring(0, tokenHtml.IndexOf('"'));
    }

    protected class ViewDataContainer : IViewDataContainer
    {
        public ViewDataDictionary ViewData
        {
            get;
            set;
        }

        public ViewDataContainer(ViewDataDictionary viewData)
        {
            this.ViewData = viewData;
        }
    }
  }
}
