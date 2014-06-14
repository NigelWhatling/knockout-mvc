namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;

    public class KnockoutHtml<TModel> : KnockoutSubContext<TModel>
    {
        private readonly ViewContext viewContext;

        public KnockoutHtml(ViewContext viewContext, KnockoutContext<TModel> context, string[] instancesNames = null, Dictionary<string, string> aliases = null)
            : base(context, instancesNames, aliases)
        {
            this.viewContext = viewContext;
        }

        private KnockoutTagBuilder<TModel> Input<TProperty>(Expression<Func<TModel, TProperty>> expression, string type, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(this.Context, "input", this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            if (expression != null)
            {
                string name = KnockoutExpressionConverter.Convert(expression, this.CreateData()); // ExpressionHelper.GetExpressionText(expression);
                if (this.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("id", this.GetSanitisedFieldName(name));
                    tagBuilder.Attr("name", this.GetFieldName(name));
                }
                else
                {
                    tagBuilder.ApplyAttributes(new { id = this.GetSanitisedFieldName(name), name = this.GetFieldName(name) });
                }

                // Add unobtrusive validation attributes
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, this.Context.HtmlHelper.ViewData);
                //ModelMetadata metadata = ModelMetadata.FromStringExpression(name, this.Context.htmlHelper.ViewData);
                IDictionary<string, object> validationAttributes = this.Context.HtmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
                tagBuilder.ApplyAttributes(validationAttributes);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                tagBuilder.ApplyAttributes(new { type });
            }

            if (expression != null)
            {
                tagBuilder.Value(expression);
            }

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Label<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(this.Context, "label", this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            if (expression != null)
            {
                string name = KnockoutExpressionConverter.Convert(expression, this.CreateData()); // ExpressionHelper.GetExpressionText(expression);
                if (this.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("for", this.GetSanitisedFieldName(name));
                }
                else
                {
                    tagBuilder.ApplyAttribute("for", this.GetSanitisedFieldName(name));
                }

                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, this.Context.HtmlHelper.ViewData);
                tagBuilder.SetInnerText(metadata.DisplayName ?? metadata.PropertyName);
            }

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> ValidationMessage<TProperty>(Expression<Func<TModel, TProperty>> expression, string validationMessage = null, object htmlAttributes = null)
        {
            string generatedTag = System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(this.Context.HtmlHelper, expression, validationMessage, htmlAttributes).ToHtmlString();
            var tagBuilder = KnockoutTagBuilder<TModel>.CreateFromHtml(this.Context, generatedTag, this.InstanceNames, this.Aliases);
            
            if (expression != null)
            {
                string name = KnockoutExpressionConverter.Convert(expression, this.CreateData()); // ExpressionHelper.GetExpressionText(expression);
                if (this.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("data-valmsg-for", this.GetFieldName(name));
                    tagBuilder.RemoveAttribute("data-valmsg-for");
                }
                else
                {
                    tagBuilder.ApplyAttribute("data-valmsg-for", this.GetFieldName(name));
                }
            }

            return tagBuilder;
        }


        private string GetFieldName(string name)
        {
            return (this.Context.ExpressionTree + "." + name).TrimStart('.').Replace("[]", "['+$index()+']");
        }

        private string GetSanitisedFieldName(string name)
        {
            string str = TagBuilder.CreateSanitizedId((this.Context.ExpressionTree + "." + name).TrimStart('.').Replace("[]", ":_index_:"));
            if (!String.IsNullOrWhiteSpace(str))
            {
                str = str.Replace(":_index_:", "_'+$index()+'_");
            }

            return str;
        }

        public KnockoutTagBuilder<TModel> TextBox<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "text", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Password<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "password", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Hidden<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "hidden", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> RadioButton<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null, object checkedValue = null)
        {
            var tagBuilder = this.Input<TProperty>(null, "radio", htmlAttributes);
            tagBuilder.Checked(expression);
            if (checkedValue != null)
            {
                tagBuilder.CheckedValue(Convert.ToString(checkedValue));
            }

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> CheckBox<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null, object checkedValue = null)
        {
            var tagBuilder = this.Input<TProperty>(null, "checkbox", htmlAttributes);
            tagBuilder.Checked(expression);
            if (checkedValue != null)
            {
                tagBuilder.CheckedValue(Convert.ToString(checkedValue));
            }

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Date<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "date", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Email<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "email", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> File<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "file", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Url<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return Input(expression, "url", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> TextArea<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "textarea", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Value(expression);
            return tagBuilder;
        }

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, options, InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            return tagBuilder;
        }

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TItem>(string customBinding, Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, options, InstanceNames, Aliases, customBinding);
            tagBuilder.ApplyAttributes(htmlAttributes);
            return tagBuilder;
        }

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, object>> optionsText = null, Expression<Func<TItem, object>> optionsValue = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, options, InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            //if (options != null)
            //    tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
            if (optionsText != null)
            {
                var data = new KnockoutExpressionData { InstanceNames = new[] { "item" } };
                tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
            }
            if (optionsValue != null)
            {
                var data = new KnockoutExpressionData { InstanceNames = new[] { "item" } };
                tagBuilder.OptionsValue("function(item) { return " + KnockoutExpressionConverter.Convert(optionsValue, data) + "; }");
            }
            return tagBuilder;
        }

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, string OptionsTextValue = null, string OptionsIdValue = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, options, InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            //if (options != null)
            //    tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
            if (!string.IsNullOrEmpty(OptionsTextValue))
            {
                tagBuilder.OptionsText(OptionsTextValue, true);
            }
            if (!string.IsNullOrEmpty(OptionsIdValue))
            {
                tagBuilder.OptionsValue(OptionsIdValue, true);
            }
            return tagBuilder;
        }

        public KnockoutSelectTagBuilder<TModel, TItem> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, object>> optionsText = null)
        {
            var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });
            return tagBuilder;
        }

        //public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        //{
        //    var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
        //    tagBuilder.ApplyAttributes(htmlAttributes);
        //    if (options != null)
        //        tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
        //    if (optionsText != null)
        //    {
        //        var data = CreateData();
        //        var keys = data.Aliases.Keys.ToList();
        //        if (!string.IsNullOrEmpty(Context.GetInstanceName()))
        //            foreach (var key in keys)
        //            {
        //                data.Aliases[Context.GetInstanceName() + "." + key] = data.Aliases[key];
        //                data.Aliases.Remove(key);
        //            }
        //        data.InstanceNames = new[] { Context.GetInstanceName(), "item" };
        //        tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
        //    }
        //    return tagBuilder;
        //}

        //public KnockoutSelectTagBuilder<TModel, TItem> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        //{
        //    var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
        //    tagBuilder.ApplyAttributes(new { multiple = "multiple" });
        //    return tagBuilder;
        //}

        public KnockoutTagBuilder<TModel> Span<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(expression);
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Span(string text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(text));
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> SpanInline(string text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(text);
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Button(string caption, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, bool noData = false)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Click(actionName, controllerName, routeValues, bindingOut: noData ? "null" : null);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Button<TProperty>(Expression<Func<TModel, TProperty>> binding, string caption, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, Expression<Func<TModel, object>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string modelOut = binding == null ? "null" : this.Context.ViewModelName + "." + KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelIn = bindingIn == null ? null : KnockoutExpressionConverter.Convert(bindingIn, CreateData());
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Click(actionName, controllerName, routeValues, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> ActionLink(string caption, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "a", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Attr("href", this.Context.GetActionUrl(actionName, controllerName, routeValues));
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> HyperlinkButton(string caption, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, bool noData = false)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "a", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.ApplyAttributes(new { href = "#" });
            tagBuilder.Click(actionName, controllerName, routeValues, bindingOut: noData ? "null" : null);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        public KnockoutFormContext<TModel> Form(string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
            var formContext = new KnockoutFormContext<TModel>(
              this.Context, this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }

        public KnockoutFormContext<TModel> Form<TProperty>(Expression<Func<TModel, TProperty>> binding, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, Expression<Func<TModel, object>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string modelOut = this.Context.ViewModelName + "." + KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelIn = bindingIn == null ? null : KnockoutExpressionConverter.Convert(bindingIn, CreateData());
            var formContext = new KnockoutFormContext<TModel>(
              this.Context, //this.Context.CreateContext<TModel>(modelOut),
              this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }

        public KnockoutFormContext<TSubModel> FormWith<TSubModel>(Expression<Func<TModel, TSubModel>> binding, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, Expression<Func<TModel, TSubModel>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string bindingName = KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelOut = this.Context.ViewModelName + "." + bindingName;
            string modelIn = bindingIn != null ? KnockoutExpressionConverter.Convert(bindingIn, CreateData()) : null;
            var formContext = new KnockoutFormContext<TSubModel>(
              this.Context.CreateContext<TSubModel>(model => binding.Compile().Invoke(model)),
              this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes, withBinding: bindingName, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }

        public KnockoutTemplateContext<TModel> Template<TProperty>(Expression<Func<TModel, TProperty>> expression, string templateId)
        {
            var templateContext = new KnockoutTemplateContext<TModel>(
              viewContext,
              this.Context,
              InstanceNames, Aliases,
              templateId);
            templateContext.WriteStart(viewContext.Writer);
            return templateContext;
        }
    }
}