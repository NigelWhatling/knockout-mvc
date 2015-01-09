namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;

    public interface IKnockoutBinding<TModel>
    {
        KnockoutBinding<TModel> Attr<TProperty>(string name, Expression<Func<TModel, TProperty>> binding);
        KnockoutBinding<TModel> Attr(string name, string binding, bool isWord);
    }

    public class KnockoutBinding<TModel> : KnockoutSubContext<TModel>, IKnockoutBinding<TModel>, IHtmlString
    {
        protected readonly TagBuilder tagBuilder;

        public KnockoutBinding(KnockoutContext<TModel> context, string tagName, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : this(context, instanceNames, aliases)
        {
            tagBuilder = new TagBuilder(tagName);
        }

        public KnockoutBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }

        internal void ApplyUnobtrusiveValidationAttributes<TProperty, TContextModel>(KnockoutContext<TContextModel> context, IKnockoutTagBuilder<TModel> tagBuilder, Expression<Func<TContextModel, TProperty>> expression)
        {
            if (expression != null)
            {
                //string name = KnockoutExpressionConverter.Convert(expression, this.CreateData()); // ExpressionHelper.GetExpressionText(expression);
                string name = ExpressionHelper.GetExpressionText(expression);
                if (context.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("id", context.GetSanitisedFieldName(name), true);
                    tagBuilder.Attr("name", context.GetFieldName(name), true);
                }
                else
                {
                    tagBuilder.ApplyAttributes(new { id = context.GetSanitisedFieldName(name), name = context.GetFieldName(name) });
                }

                // Add unobtrusive validation attributes
                ModelMetadata metadata = context.GetModelMetadata(expression);
                IDictionary<string, object> validationAttributes = context.HtmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
                tagBuilder.ApplyAttributes(validationAttributes);
            }
        }

        public KnockoutBinding<TModel> Name<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return this.Name(this.Context, expression);
        }

        public KnockoutBinding<TModel> Name<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            IKnockoutTagBuilder<TModel> tagBuilder = this as IKnockoutTagBuilder<TModel>;
            if (tagBuilder == null)
            {
                throw new NotSupportedException();
            }

            string name = ExpressionHelper.GetExpressionText(expression);
            if (context.GetFieldName(name).Contains("$index()"))
            {
                tagBuilder.Attr("id", context.GetSanitisedFieldName(name), true);
                tagBuilder.Attr("name", context.GetFieldName(name), true);
            }
            else
            {
                tagBuilder.ApplyAttributes(new { id = context.GetSanitisedFieldName(name), name = context.GetFieldName(name) });
            }

            return this;
        }

        public KnockoutBinding<TModel> UnobtrusiveValidationAttributes<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            IKnockoutTagBuilder<TModel> tagBuilder = this as IKnockoutTagBuilder<TModel>;
            if (tagBuilder == null)
            {
                throw new NotSupportedException();
            }

            this.ApplyUnobtrusiveValidationAttributes(this.Context, tagBuilder, expression);
            return this;
        }

        public KnockoutBinding<TModel> UnobtrusiveValidationAttributes<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            IKnockoutTagBuilder<TModel> tagBuilder = this as IKnockoutTagBuilder<TModel>;
            if (tagBuilder == null)
            {
                throw new NotSupportedException();
            }

            this.ApplyUnobtrusiveValidationAttributes(context, tagBuilder, expression);
            return this;
        }

        // *** Controlling text and appearance ***

        // Visible
        public KnockoutBinding<TModel> Visible<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("visible", expression));
            return this;
        }

        public KnockoutBinding<TModel> Visible<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("visible", expression, context));
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
            Items.Add(new KnockoutBindingItem("text", expression));
            return this;
        }

        public KnockoutBinding<TModel> Text<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("text", expression, context));
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
            Items.Add(new KnockoutBindingItem("html", binding));
            return this;
        }

        public KnockoutBinding<TModel> Html<TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, string>> binding)
        {
            Items.Add(new KnockoutBindingItem("html", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Html(Expression<Func<TModel, Expression<Func<string>>>> binding)
        {
            Items.Add(new KnockoutBindingItem("html", binding));
            return this;
        }

        public KnockoutBinding<TModel> Html(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("html", binding, false));
            return this;
        }

        // *** Working with form fields ***
        public KnockoutBinding<TModel> Value<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("value", expression));
            return this;
        }

        public KnockoutBinding<TModel> Value<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("value", expression, context));
            return this;
        }

        public KnockoutBinding<TModel> Value(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("value", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> TextInput<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("textInput", expression));
            return this;
        }

        public KnockoutBinding<TModel> TextInput<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem("textInput", expression, context));
            return this;
        }

        public KnockoutBinding<TModel> TextInput(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("textInput", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Disable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem("disable", binding));
            return this;
        }

        public KnockoutBinding<TModel> Disable<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem("disable", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Disable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("disable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Enable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem("enable", binding));
            return this;
        }

        public KnockoutBinding<TModel> Enable<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem("enable", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Enable(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("enable", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> Checked<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("checked", binding));
            return this;
        }

        public KnockoutBinding<TModel> Checked<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("checked", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> Checked(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checked", binding, isWord));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("checkedValue", binding));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("checkedValue", binding, context));
            return this;
        }

        public KnockoutBinding<TModel> CheckedValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("checkedValue", text, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> Options<TItem>(Expression<Func<TModel, ICollection<TItem>>> expression)
        {
            KnockoutSelectBinding<TModel, TItem> binding = new KnockoutSelectBinding<TModel, TItem>(this.Context, this.Context.CreateData().InstanceNames, this.Context.CreateData().Aliases);
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingItem("options", expression));
            return binding;
        }

        public KnockoutSelectBinding<TParent, TItem> Options<TItem, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, ICollection<TItem>>> expression)
        {
            KnockoutSelectBinding<TParent, TItem> binding = new KnockoutSelectBinding<TParent, TItem>(context, context.CreateData().InstanceNames, context.CreateData().Aliases);
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingItem("options", expression));
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
            Items.Add(new KnockoutBindingItem("hasfocus", binding));
            return this;
        }

        // *** Complex ***
        public KnockoutBinding<TModel> Css<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("css").Add(new KnockoutBindingItem(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Style<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("style").Add(new KnockoutBindingItem(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Attr<TProperty>(string name, Expression<Func<TModel, TProperty>> binding)
        {
            ComplexItem("attr").Add(new KnockoutBindingItem(name, binding));
            return this;
        }

        public KnockoutBinding<TModel> Attr(string name, string binding, bool isWord = true)
        {
            ComplexItem("attr").Add(new KnockoutBindingStringItem(name, binding, isWord));
            return this;
        }

        // *** Events ***
        public virtual KnockoutBinding<TModel> Event(string eventName, string actionName, string controllerName, object routeValues,
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
            this.Event(eventName, sb.ToString());
            return this;
        }

        public virtual KnockoutBinding<TModel> Event(string eventName, string propertyName)
        {
            if (eventName == "click" || eventName == "submit")
            {
                this.Items.Add(new KnockoutBindingStringItem(eventName, propertyName, false));
            }
            else
            {
                this.ComplexItem("event").Add(new KnockoutBindingStringItem(eventName, propertyName, false));
            }

            return this;
        }

        public KnockoutBinding<TModel> Click(string actionName, string controllerName, object routeValues = null, string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            return Event("click", actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings);
        }

        public KnockoutBinding<TModel> Click(string propertyName)
        {
            return Event("click", propertyName);
        }

        public KnockoutBinding<TModel> Submit(string actionName, string controllerName, object routeValues = null, string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            return Event("submit", actionName, controllerName, routeValues, bindingOut: bindingOut, bindingIn: bindingIn, settings: settings);
        }

        public KnockoutBinding<TModel> Submit(string propertyName)
        {
            return Event("submit", propertyName);
        }

        // *** Flow Control *** 

        public KnockoutBinding<TModel> ForEach<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem { Name = "foreach", Expression = binding });
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
            Items.Add(new KnockoutBindingItem { Name = "if", Expression = binding });
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
                this.ComplexItem("template").Add(new KnockoutBindingItem { Name = "data", Expression = dataBinding });
            }

            if (ifBinding != null)
            {
                this.ComplexItem("template").Add(new KnockoutBindingItem { Name = "if", Expression = dataBinding });
            }

            if (foreachBinding != null)
            {
                this.ComplexItem("template").Add(new KnockoutBindingItem { Name = "foreach", Expression = dataBinding });
            }

            return this;
        }

        // *** Custom ***    
        public KnockoutBinding<TModel> Custom<TProperty>(string binding, Expression<Func<TModel, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem(binding, expression));
            return this;
        }

        public KnockoutBinding<TModel> Custom<TProperty, TParent>(KnockoutContext<TParent> context, string binding, Expression<Func<TParent, TProperty>> expression)
        {
            Items.Add(new KnockoutBindingItem(binding, expression, context));
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
