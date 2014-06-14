namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq.Expressions;
    using System.Web;

    public class KnockoutBinding<TModel> : KnockoutSubContext<TModel>, IHtmlString
    {
        public KnockoutBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }

        // *** Controlling text and appearance ***

        // Visible
        public KnockoutBinding<TModel> Visible<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("visible", expression));
            return this;
        }

        public KnockoutBinding<TModel> Visible<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("visible", expression, context));
            return this;
        }

        public KnockoutBinding<TModel> Visible(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("visible", binding, false));
            return this;
        }

        // Text
        public KnockoutBinding<TModel> Text<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("text", expression));
            return this;
        }

        public KnockoutBinding<TModel> Text<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("text", expression, context));
            return this;
        }

        public KnockoutBinding<TModel> Text(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("text", binding, false));
            return this;
        }

        // Html
        public KnockoutBinding<TModel> Html(Expression<Func<TModel, string>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, string>("html", binding));
            return this;
        }

        public KnockoutBinding<TModel> Html<TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, string>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, string>("html", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Html(Expression<Func<TModel, Expression<Func<string>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<string>>>("html", binding));
            return this;
        }

        // *** Working with form fields ***
        public KnockoutBinding<TModel> Value<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("value", expression));
            return this;
        }

        public KnockoutBinding<TModel> Value<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("value", expression, context));
            return this;
        }

        public KnockoutBinding<TModel> Value(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("value", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Disable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool>("disable", binding));
            return this;
        }

        public KnockoutBinding<TModel> Disable<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, bool>("disable", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Disable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("disable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Enable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool>("enable", binding));
            return this;
        }

        public KnockoutBinding<TModel> Enable<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, bool>("enable", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Enable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("enable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Checked<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("checked", binding));
            return this;
        }

        public KnockoutBinding<TModel> Checked<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("checked", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Checked(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checked", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("checkedValue", binding));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("checkedValue", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checkedValue", text, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> Options<TItem>(Expression<Func<TModel, IList<TItem>>> expression)
        {
            KnockoutSelectBinding<TModel, TItem> binding = new KnockoutSelectBinding<TModel, TItem>(this.Context, this.Context.CreateData().InstanceNames, this.Context.CreateData().Aliases);
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingItem<TModel, IList<TItem>>("options", expression));
            return binding;
        }

        public KnockoutSelectBinding<TParent, TItem> Options<TItem, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, IList<TItem>>> expression)
        {
            KnockoutSelectBinding<TParent, TItem> binding = new KnockoutSelectBinding<TParent, TItem>(context, context.CreateData().InstanceNames, context.CreateData().Aliases);
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingItem<TParent, IList<TItem>>("options", expression));
            return binding;

            //Items.Add(new KnockoutBindingItem<TParent, IEnumerable>("options", binding, context));
            //return this;
        }

        public KnockoutBinding<TModel> Options(string text, bool isWord = false)
        {
            KnockoutSelectBinding<TModel, object> binding = new KnockoutSelectBinding<TModel, object>(this.Context, this.Context.CreateData().InstanceNames, this.Context.CreateData().Aliases);
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingStringItem("options", text, isWord));
            return binding;

            //Items.Add(new KnockoutBindingStringItem("options", binding, isWord));
            //return this;
        }

        public KnockoutBinding<TModel> UniqueName()
        {
            Items.Add(new KnockoutBindingStringItem("uniqueName", "true", false));
            return this;
        }

        public KnockoutBinding<TModel> ValueUpdate(KnockoutValueUpdateKind kind)
        {
            Items.Add(new KnockoutBindingStringItem("valueUpdate", Enum.GetName(typeof(KnockoutValueUpdateKind), kind).ToLower()));
            return this;
        }

        public KnockoutBinding<TModel> HasFocus<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("hasfocus", binding));
            return this;
        }

        // *** Complex ***
        public KnockoutBinding<TModel> Css<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("css").Add(new KnockoutBindingItem<TModel, TProperty>(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Style<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("style").Add(new KnockoutBindingItem<TModel, TProperty>(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Attr<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("attr").Add(new KnockoutBindingItem<TModel, TProperty>(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Attr(string name, string binding)
        {
            ComplexItem("attr").Add(new KnockoutBindingStringItem(name, binding));
            return this;
        }

        // *** Events ***
        protected virtual KnockoutBinding<TModel> Event(string eventName, string actionName, string controllerName, object routeValues,
            string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            var sb = new StringBuilder();
            
            if (eventName == "submit")
            {
                sb.Append("function(form) { ");
                sb.Append(Context.FormServerAction(actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings));
            }
            else
            {
                sb.Append("function() { ");
                sb.Append(Context.ServerAction(actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings));
            }

            sb.Append(" }");
            Items.Add(new KnockoutBindingStringItem(eventName, sb.ToString(), false));
            return this;
        }

        protected virtual KnockoutBinding<TModel> Event2(string eventName, string propertyName)
        {
            Items.Add(new KnockoutBindingStringItem(eventName, propertyName, false));
            return this;
        }

        public KnockoutBinding<TModel> Click(string actionName, string controllerName, object routeValues = null, string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            return Event("click", actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings);
        }

        public KnockoutBinding<TModel> Click(string propertyName)
        {
            return Event2("click", propertyName);
        }

        public KnockoutBinding<TModel> Submit(string actionName, string controllerName, object routeValues = null, string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            return Event("submit", actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings);
        }

        // *** Flow Control *** 

        public KnockoutBinding<TModel> ForEach<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty> { Name = "foreach", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> ForEach(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("foreach", binding, false));
            return this;
        }

        public KnockoutBinding<TModel> ForEach()
        {
            KnockoutCommonRegionContext<TModel> context = this.Context as KnockoutCommonRegionContext<TModel>;
            if (context == null)
            {
                throw new InvalidOperationException("ForEach() must be used within ForEachContext.");
            }

            Items.Add(new KnockoutBindingStringItem("foreach", context.Expression, false));
            return this;
        }

        public KnockoutBinding<TModel> If<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty> { Name = "if", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> If(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("if", binding, isWord));
            return this;
        }

        // *** Templating ***
        public KnockoutBinding<TModel> Template(string templateId, Expression<Func<TModel, object>> dataBinding = null, Expression<Func<TModel, object>> ifBinding = null, Expression<Func<TModel, object>> foreachBinding = null)
        {
            this.ComplexItem("template").Add(new KnockoutBindingStringItem("name", templateId, true));

            if (dataBinding != null)
            {
                this.ComplexItem("template").Add(new KnockoutBindingItem<TModel, object> { Name = "data", Expression = dataBinding });
            }

            if (ifBinding != null)
            {
                this.ComplexItem("template").Add(new KnockoutBindingItem<TModel, object> { Name = "if", Expression = dataBinding });
            }

            if (foreachBinding != null)
            {
                this.ComplexItem("template").Add(new KnockoutBindingItem<TModel, object> { Name = "foreach", Expression = dataBinding });
            }

            return this;
        }

        // *** Custom ***    
        public KnockoutBinding<TModel> Custom<TProperty>(string binding, Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>(binding, expression));
            return this;
        }

        public KnockoutBinding<TModel> Custom<TProperty, TParent>(KnockoutContext<TParent> context, string binding, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>(binding, expression, context));
            return this;
        }

        public KnockoutBinding<TModel> Custom(string binding, string value, bool isProperty = false)
        {
            Items.Add(new KnockoutBindingStringItem(binding, value, needQuotes: isProperty));
            return this;
        }

        // *** Common ***

        private readonly List<KnockoutBindingItem> items = new List<KnockoutBindingItem>();

        private readonly Dictionary<string, KnockoutBindingComplexItem> complexItems = new Dictionary<string, KnockoutBindingComplexItem>();

        public List<KnockoutBindingItem> Items
        {
            get
            {
                return items;
            }
        }

        private KnockoutBindingComplexItem ComplexItem(string name)
        {
            if (!complexItems.ContainsKey(name))
            {
                complexItems[name] = new KnockoutBindingComplexItem { Name = name };
                items.Add(complexItems[name]);
            }
            return complexItems[name];
        }

        public virtual string ToHtmlString()
        {
            if (this.Items.Count == 0)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("data-bind=\"");
            sb.Append(this.BindingAttributeContent());
            sb.Append("\"");
            return sb.ToString();
        }

        public string BindingAttributeContent()
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (var item in Items)
            {
                if (!item.IsValid())
                {
                    continue;
                }

                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }

                if (item.Context == null)
                {
                    sb.Append(item.GetKnockoutExpression(this.CreateData()));
                }
                else
                {
                    sb.Append(item.GetKnockoutExpression(item.Context.CreateData()));
                }
            }
            return sb.ToString();
        }
    }
}
