namespace KnockoutMvc
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;
    using System.Web;
    using System.Web.Routing;
    using System.Linq.Expressions;
    using Newtonsoft.Json;

    public interface IKnockoutContext
    {
        string GetInstanceName();
        string GetIndex();
        KnockoutExpressionData CreateData();
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

        public bool UseAntiForgeryToken
        {
            get;
            set;
        }

        protected List<IKnockoutContext> ContextStack { get; set; }

        public KnockoutContext(KnockoutContext<TModel> context)
            : this(context.HtmlHelper)
        {
        }

        public KnockoutContext(HtmlHelper<TModel> htmlHelper)
        {
            this._HtmlHelper = htmlHelper;
            this.viewContext = htmlHelper.ViewContext;
            this.ContextStack = new List<IKnockoutContext>();
            this.UseAntiForgeryToken = true;
        }

        public KnockoutContext(HtmlHelper<TModel> htmlHelper, string modelName)
            : this(htmlHelper)
        {
            this.ViewModelName = modelName;
        }

        public KnockoutContext<TModel2> CreateContext<TModel2>(Func<TModel, TModel2> modelFunc)
        {
            ViewContext currentViewContext = this.HtmlHelper.ViewContext;
            TModel2 model = modelFunc.Invoke((TModel)this.HtmlHelper.ViewDataContainer.ViewData.Model);

            ViewContext context = new ViewContext(
                    currentViewContext,
                    currentViewContext.View,
                    new ViewDataDictionary(model),
                    currentViewContext.TempData,
                    currentViewContext.Writer);

            return new KnockoutContext<TModel2>(new HtmlHelper<TModel2>(context, new KnockoutViewDataContainer<TModel2>(model)));
        }

        //public KnockoutContext<TModel2> CreateContext<TModel2>(string modelName)
        //{
        //    return new KnockoutContext<TModel2>(this.htmlHelper, modelName);
        //}

        private readonly ViewContext viewContext;
        private readonly HtmlHelper<TModel> _HtmlHelper;

        private bool isInitialized;

        internal HtmlHelper<TModel> HtmlHelper
        {
            get
            {
                return this._HtmlHelper;
            }
        }

        public string ExpressionTree
        {
            get
            {
                string tree = String.Empty;
                for (int i = this.ContextStack.Count - 1; i >= 0; i--)
                {
                    IKnockoutCommonRegionContext context = this.ContextStack[i] as IKnockoutCommonRegionContext;
                    if (context == null)
                    {
                        continue;
                    }

                    if (tree.Length > 0)
                    {
                        tree += ".";
                    }

                    tree += context.Expression;

                    if (this.ContextStack[i] is IKnockoutForeachContext)
                    {
                        tree += "[]";
                    }
                }

                return tree;
            }
        }

        private string GetInitializeData(TModel model, string searchScope, bool needBinding, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Error)
        {
            if (isInitialized)
            {
                return String.Empty;
            }

            isInitialized = true;
            KnockoutUtilities.ConvertData(model);
            this.model = model;

            var sb = new StringBuilder();

            var json = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = referenceLoopHandling });

            sb.AppendLine(@"<script type=""text/javascript"">");
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
            sb.AppendLine(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
            sb.AppendLine("ko.mvc.init(" + ViewModelName + ", \"" + this.GetAntiForgeryToken() + "\");");
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

        public HtmlString Initialize(TModel model, string searchScope = null)
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

        public HtmlString LazyApply(TModel model, string actionName, string controllerName, object routeValues = null,
            string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null, string searchScope = null)
        {
            var sb = new StringBuilder();

            sb.Append("function (data) { ");

            string mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
            if (mappingData == "{}")
                sb.Append(string.Format("{0} = ko.mapping.fromJS(data); ", bindingIn ?? ViewModelName));
            else
            {
                sb.Append(string.Format("var {0}MappingData = {1}; ", ViewModelName, mappingData));
                sb.Append(string.Format("{0} = ko.mapping.fromJS(data, {0}MappingData); ", bindingIn ?? ViewModelName));
            }
            sb.Append(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
            if (searchScope == null)
            {
                sb.Append(string.Format("ko.applyBindings({0});", ViewModelName));
            }
            else
            {
                sb.Append(string.Format(@"ko.applyBindings({0}, $(""{1}"").get(0));", ViewModelName, searchScope));
            }

            sb.Append("}");

            string overrideFunction = sb.ToString();
            sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript""> ");
            if (this.UseAntiForgeryToken)
            {
                sb.AppendLine(this.GetAntiForgeryToken());
            }

            sb.AppendLine(String.Format("var {0};", ViewModelName));
            sb.Append("$(document).ready(function() { ");
            sb.Append(this.ServerAction(actionName, controllerName, routeValues: routeValues, bindingIn: bindingIn, bindingOut: bindingOut, settings: settings, successOverride: overrideFunction));
            sb.AppendLine(" });");
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
            var regionContext = new KnockoutForeachContext<TItem>(this.CreateContext<TItem>(model => { return default(TItem); }), expression);
            regionContext.WriteStart(viewContext.Writer);
            regionContext.ContextStack = ContextStack;
            ContextStack.Add(regionContext);
            return regionContext;
        }

        public KnockoutForeachContext<TItem> ForeachContext<TItem>(Expression<Func<TModel, IList<TItem>>> binding)
        {
            var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
            var regionContext = new KnockoutForeachContext<TItem>(this.CreateContext<TItem>(model => { return default(TItem); }), expression, false);
            regionContext.WriteStart(viewContext.Writer);
            regionContext.ContextStack = ContextStack;
            ContextStack.Add(regionContext);
            return regionContext;
        }

        public KnockoutWithContext<TItem> With<TItem>(Expression<Func<TModel, TItem>> binding)
        {
            Func<TModel, TItem> func = binding.Compile();

            var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
            //var regionContext = new KnockoutWithContext<TItem>(this.CreateContext<TItem>(), expression);
            var regionContext = new KnockoutWithContext<TItem>(this.CreateContext<TItem>(model => { return func.Invoke(model); }), expression);
            regionContext.WriteStart(viewContext.Writer);
            regionContext.ContextStack = ContextStack;
            ContextStack.Add(regionContext);
            return regionContext;
        }

        public KnockoutIfContext<TModel> If(Expression<Func<TModel, bool>> binding)
        {
            var regionContext = new KnockoutIfContext<TModel>(this, KnockoutExpressionConverter.Convert(binding));
            regionContext.InStack = false;
            regionContext.WriteStart(viewContext.Writer);
            return regionContext;
        }

        public KnockoutFormContext<TSubModel> FormContext<TSubModel>(Expression<Func<TModel, TSubModel>> binding, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, Expression<Func<TModel, TSubModel>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string bindingName = KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelOut = this.ViewModelName + "." + bindingName;
            string modelIn = bindingIn != null ? KnockoutExpressionConverter.Convert(bindingIn, CreateData()) : null;
            var formContext = new KnockoutFormContext<TSubModel>(
              this.CreateContext<TSubModel>(model => binding.Compile().Invoke(model)),
              this.CreateData().InstanceNames, this.CreateData().Aliases,
              actionName, controllerName, routeValues, htmlAttributes, withBinding: bindingName, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            formContext.WriteStart(viewContext.Writer);
            formContext.ContextStack = this.ContextStack;
            this.ContextStack.Add(formContext);
            return formContext;
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
                //return new KnockoutScript<TModel>(viewContext, this, CreateData().InstanceNames, CreateData().Aliases);
                return new KnockoutScript<TModel>(viewContext, this, new string[] { ViewModelName }, null);
            }
        }

        public MvcHtmlString FormServerAction(string actionName, string controllerName, object routeValues = null,
            string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null, string successOverride = null)
        {
            return ServerActionImpl(true, actionName, controllerName, routeValues, bindingOut, bindingIn, settings, successOverride);
        }

        public MvcHtmlString ServerAction(string actionName, string controllerName, object routeValues = null,
            string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null, string successOverride = null)
        {
            return ServerActionImpl(false, actionName, controllerName, routeValues, bindingOut, bindingIn, settings, successOverride);
        }

        public MvcHtmlString ServerActionImpl(bool isForm, string actionName, string controllerName, object routeValues = null,
            string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null, string successOverride = null)
        {
            bool settingsProvided = settings != null;
            if (!settingsProvided)
            {
                settings = new KnockoutExecuteSettings();
            }

            string url = this.GetActionUrl(actionName, controllerName, routeValues);
            StringBuilder sb = new StringBuilder();
            sb.Append(this.ViewModelName + ".");

            if (isForm)
            {
                sb.AppendFormat("submitForm(form, {0}, '{1}'", bindingOut ?? this.ViewModelName, url);
            }
            else
            {
                sb.AppendFormat("executeOnServer({0}, '{1}'", bindingOut ?? this.ViewModelName, url);
            }

            if (bindingIn != null || settingsProvided || successOverride != null)
            {
                sb.AppendFormat(", {0}, {1}, {2}",
                  bindingIn ?? "null",
                  settingsProvided ? settings.ToString() : "null",
                  successOverride != null ? successOverride : "null");
            }

            sb.Append(");");

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

            return new MvcHtmlString(exec);
        }

        public string GetActionUrl(string actionName, string controllerName, object routeValues)
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

            string url = Url().Action(actionName, controllerName, newRoutes) ?? "/";
            url = url.Replace("%28", "(")
                     .Replace("%29", ")")
                     .Replace("%24", "$");

            foreach (string token in tokens.Keys)
            {
                url = url.Replace(token, "'+" + tokens[token].ToHtmlString() + "+'");
            }

            if (url.EndsWith("+''"))
            {
                url = url.Substring(0, url.Length - 3);
            }

            return url;
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
    }

    public class KnockoutViewDataContainer<TModel> : IViewDataContainer
    {
        public ViewDataDictionary ViewData
        {
            get;
            set;
        }

        public KnockoutViewDataContainer(TModel model)
        {
            this.ViewData = new ViewDataDictionary<TModel>(model);
        }
    }
}
