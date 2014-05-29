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

        private KnockoutTagBuilder<TModel> Input(Expression<Func<TModel, object>> text, string type, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(this.Context, "input", this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            if (text != null)
            {
                string name = TagBuilder.CreateSanitizedId(ExpressionHelper.GetExpressionText(text));
                tagBuilder.ApplyAttributes(new { id = name, name = name });

                // Add unobtrusive validation attributes
                //ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, object>(text, this.Context.htmlHelper.ViewData);
                ModelMetadata metadata = ModelMetadata.FromStringExpression(name, this.Context.htmlHelper.ViewData);
                IDictionary<string, object> validationAttributes = this.Context.htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
                tagBuilder.ApplyAttributes(validationAttributes);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                tagBuilder.ApplyAttributes(new { type });
            }

            if (text != null)
            {
                tagBuilder.Value(text);
            }

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Label(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(this.Context, "label", this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            if (text != null)
            {
                string name = ExpressionHelper.GetExpressionText(text);
                tagBuilder.ApplyAttributes(new { @for = name });
            }

            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> TextBox(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "text", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Password(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "password", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Hidden(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "hidden", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> RadioButton(Expression<Func<TModel, object>> @checked, object htmlAttributes = null, object checkedValue = null)
        {
            var tagBuilder = Input(null, "radio", htmlAttributes);
            tagBuilder.Checked(@checked);
            if (checkedValue != null)
            {
                tagBuilder.CheckedValue(Convert.ToString(checkedValue));
            }

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> CheckBox(Expression<Func<TModel, object>> @checked, object htmlAttributes = null, object checkedValue = null)
        {
            var tagBuilder = Input(null, "checkbox", htmlAttributes);
            tagBuilder.Checked(@checked);
            if (checkedValue != null)
            {
                tagBuilder.CheckedValue(Convert.ToString(checkedValue));
            }

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Date(Expression<Func<TModel, object>> date, object htmlAttributes = null)
        {
            return Input(date, "date", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Email(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "email", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> File(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "file", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Url(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "url", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> TextArea(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "textarea", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Value(text);
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, object>> optionsText = null, Expression<Func<TItem, object>> optionsValue = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
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

        public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, string OptionsTextValue = null, string OptionsIdValue = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
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

        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, object>> optionsText = null)
        {
            var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
            if (optionsText != null)
            {
                var data = CreateData();
                var keys = data.Aliases.Keys.ToList();
                if (!string.IsNullOrEmpty(Context.GetInstanceName()))
                    foreach (var key in keys)
                    {
                        data.Aliases[Context.GetInstanceName() + "." + key] = data.Aliases[key];
                        data.Aliases.Remove(key);
                    }
                data.InstanceNames = new[] { Context.GetInstanceName(), "item" };
                tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
            }
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        {
            var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Span(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(text);
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

        public KnockoutTagBuilder<TModel> Button(Expression<Func<TModel, object>> binding, string caption, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null, Expression<Func<TModel, object>> bindingIn = null, KnockoutExecuteSettings settings = null)
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
              viewContext,
              this.Context, this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }

        public KnockoutFormContext<TModel> Form(Expression<Func<TModel, object>> binding, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null,
          Expression<Func<TModel, object>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string modelOut = this.Context.ViewModelName + "." + KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelIn = bindingIn == null ? null : KnockoutExpressionConverter.Convert(bindingIn, CreateData());
            var formContext = new KnockoutFormContext<TModel>(
              viewContext,
              this.Context, //this.Context.CreateContext<TModel>(modelOut),
              this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }

        /*
        public KnockoutFormContext<TSubModel> Form<TSubModel>(Expression<Func<TModel, TSubModel>> binding, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null,
            Expression<Func<TModel, TSubModel>> bindingIn = null, KnockoutExecuteSettings settings = null)
        {
            string modelOut = this.Context.ViewModelName + "." + KnockoutExpressionConverter.Convert(binding, CreateData());
            string modelIn = bindingIn != null ? KnockoutExpressionConverter.Convert(bindingIn, CreateData()) : null;
            var formContext = new KnockoutFormContext<TSubModel>(
              viewContext,
              this.Context.CreateContext<TSubModel>(modelOut),
              this.InstanceNames, this.Aliases,
              actionName, controllerName, routeValues, htmlAttributes, bindingOut: modelOut, bindingIn: modelIn, settings: settings);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }
        */

        public KnockoutTemplateContext<TModel> Template(Expression<Func<TModel, object>> binding, string templateId)
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