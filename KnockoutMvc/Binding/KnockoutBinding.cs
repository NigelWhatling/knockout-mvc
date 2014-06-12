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
        public KnockoutBinding<TModel> Visible(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "visible", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Visible(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("visible", binding, false));
            return this;
        }

        // Text
        public KnockoutBinding<TModel> Text(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "text", Expression = binding });
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
            Items.Add(new KnockoutBindingItem<TModel, string> { Name = "html", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Html(Expression<Func<TModel, Expression<Func<string>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<string>>> { Name = "html", Expression = binding });
            return this;
        }

        // *** Working with form fields ***
        public KnockoutBinding<TModel> Value(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "value", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Value(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("value", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Disable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool> { Name = "disable", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Disable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("disable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Enable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool> { Name = "enable", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Enable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("enable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Checked(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "checked", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Checked(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checked", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "checkedValue", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checkedValue", text, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Options(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable> { Name = "options", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Options(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("options", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> SelectedOptions(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable> { Name = "selectedOptions", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> SelectedOptions(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("selectedOptions", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> OptionsCaption(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "optionsCaption", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> OptionsCaption(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsCaption", binding));
            return this;
        }

        public KnockoutBinding<TModel> OptionsText(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "optionsText", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> OptionsText(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsText", text, isWord));
            return this;
        }

        public KnockoutBinding<TModel> OptionsValue(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "optionsValue", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> OptionsValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsValue", text, isWord));
            return this;
        }

        public KnockoutBinding<TModel> ValueAllowUnset(bool isAllowed = true)
        {
            Items.Add(new KnockoutBindingStringItem("valueAllowUnset", isAllowed.ToString().ToLower(), false));
            return this;
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

        public KnockoutBinding<TModel> HasFocus(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "hasfocus", Expression = binding });
            return this;
        }

        // *** Complex ***
        public KnockoutBinding<TModel> Css(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("css").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Style(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("style").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Attr(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("attr").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
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

        public KnockoutBinding<TModel> ForEach(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "foreach", Expression = binding });
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

        public KnockoutBinding<TModel> If(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "if", Expression = binding });
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
        public KnockoutBinding<TModel> Custom(string name, string value, bool isProperty = false)
        {
            Items.Add(new KnockoutBindingStringItem(name, value, needQuotes: isProperty));
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
                    continue;
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(item.GetKnockoutExpression(CreateData()));
            }
            return sb.ToString();
        }
    }
}
