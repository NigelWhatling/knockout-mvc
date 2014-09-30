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
            tagBuilder.ApplyUnobtrusiveValidationAttributes(this.Context, tagBuilder, expression);

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
                if (this.Context.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("for", this.Context.GetSanitisedFieldName(name));
                }
                else
                {
                    tagBuilder.ApplyAttribute("for", this.Context.GetSanitisedFieldName(name));
                }

                ModelMetadata metadata = this.Context.GetModelMetadata(expression);
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
                if (this.Context.GetFieldName(name).Contains("$index()"))
                {
                    tagBuilder.Attr("data-valmsg-for", this.Context.GetFieldName(name));
                    tagBuilder.RemoveAttribute("data-valmsg-for");
                }
                else
                {
                    tagBuilder.ApplyAttribute("data-valmsg-for", this.Context.GetFieldName(name));
                }
            }

            return tagBuilder;
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
            tagBuilder.ApplyUnobtrusiveValidationAttributes(this.Context, tagBuilder, expression);
            tagBuilder.Value(expression);
            return tagBuilder;
        }

        #region DropDownList

        // ko.Html.DropDownList([htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, object> DropDownList(object htmlAttributes = null)
        {
            KnockoutSelectTagBuilder<TModel, object> tagBuilder = new KnockoutSelectTagBuilder<TModel, object>(this.Context, this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            return tagBuilder;
        }

        // ko.Html.DropDownList(optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TItem>(
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<object, TModel, TItem, object, object>(null, null, optionsExpression, null, null, htmlAttributes);
        }

        // ko.Html.DropDownList(valueExpression, optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TProperty, TItem>(
            Expression<Func<TModel, TProperty>> valueExpression,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<TProperty, TModel, TItem, object, object>(valueExpression, null, optionsExpression, null, null, htmlAttributes);
        }

        // ko.Html.DropDownList(valueExpression, optionsContext, optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TProperty, TOptionsModel, TItem>(
            Expression<Func<TModel, TProperty>> valueExpression,
            KnockoutContext<TOptionsModel> optionsContext,
            Expression<Func<TOptionsModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<TProperty, TOptionsModel, TItem, object, object>(valueExpression, null, optionsExpression, null, null, htmlAttributes);
        }
        
        // ko.Html.DropDownList(valueExpression, optionsExpression, [optionsTextExpression], [optionsValueExpression], [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TProperty, TItem, TTextProperty, TValueProperty>(
            Expression<Func<TModel, TProperty>> valueExpression,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            Expression<Func<TItem, TTextProperty>> optionsTextExpression = null,
            Expression<Func<TItem, TValueProperty>> optionsValueExpression = null,
            object htmlAttributes = null)
        {
            return this.BuildSelect(valueExpression, this.Context, optionsExpression, optionsTextExpression, optionsValueExpression, htmlAttributes);
        }

        // ko.Html.DropDownList(valueExpression, optionsContext, optionsExpression, [optionsTextExpression], [optionsValueExpression], [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TProperty, TOptionsModel, TItem, TTextProperty, TValueProperty>(
            Expression<Func<TModel, TProperty>> valueExpression,
            KnockoutContext<TOptionsModel> optionsContext,
            Expression<Func<TOptionsModel, IList<TItem>>> optionsExpression,
            Expression<Func<TItem, TTextProperty>> optionsTextExpression = null,
            Expression<Func<TItem, TValueProperty>> optionsValueExpression = null,
            object htmlAttributes = null)
        {
            return this.BuildSelect(valueExpression, optionsContext, optionsExpression, optionsTextExpression, optionsValueExpression, htmlAttributes);
        }

        // ko.Html.DropDownList(optionsRaw, [htmlAttributes])

        //public KnockoutSelectTagBuilder<TModel, TModel, object> DropDownList(
        //    string optionsRaw,
        //    object htmlAttributes = null)
        //{
        //    var tagBuilder = new KnockoutSelectTagBuilder<TModel, TModel, object>(this.Context, optionsRaw, this.InstanceNames, this.Aliases);
        //    tagBuilder.ApplyAttributes(htmlAttributes);
        //    return tagBuilder;
        //}

        // Needed??
        // ko.Html.DropDownList(valueExpression, optionsExpression, optionsTextRaw, optionsIdRaw, [htmlAttributes])
        
        public KnockoutSelectTagBuilder<TModel, TItem> DropDownList<TProperty, TItem>(
            Expression<Func<TModel, TProperty>> valueExpression,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            string optionsTextRaw = null,
            string optionsIdRaw = null,
            object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, this.Context, optionsExpression, this.InstanceNames, this.Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.ApplyUnobtrusiveValidationAttributes(this.Context, tagBuilder, valueExpression);
            
            if (!string.IsNullOrEmpty(optionsTextRaw))
            {
                tagBuilder.OptionsText(optionsTextRaw, true);
            }

            if (!string.IsNullOrEmpty(optionsIdRaw))
            {
                tagBuilder.OptionsValue(optionsIdRaw, true);
            }

            if (valueExpression != null)
            {
                tagBuilder.Value(valueExpression);
            }

            return tagBuilder;
        }
        
        private KnockoutSelectTagBuilder<TModel, TItem> BuildSelect<TProperty, TOptionsModel, TItem, TTextProperty, TValueProperty>(
            Expression<Func<TModel, TProperty>> valueExpression,
            KnockoutContext<TOptionsModel> optionsContext,
            Expression<Func<TOptionsModel, IList<TItem>>> optionsExpression,
            Expression<Func<TItem, TTextProperty>> optionsTextExpression = null,
            Expression<Func<TItem, TValueProperty>> optionsValueExpression = null,
            object htmlAttributes = null,
            string customBinding = null)
        {
            KnockoutSelectTagBuilder<TModel, TItem> tagBuilder = new KnockoutSelectTagBuilder<TModel, TItem>(this.Context, optionsContext, optionsExpression, this.InstanceNames, this.Aliases, customBinding);

            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.ApplyUnobtrusiveValidationAttributes(this.Context, tagBuilder, valueExpression);

            if (optionsTextExpression != null)
            {
                tagBuilder.OptionsText(optionsTextExpression);
            }

            if (optionsValueExpression != null)
            {
                tagBuilder.OptionsValue(optionsValueExpression);
            }

            if (valueExpression != null)
            {
                tagBuilder.Value(valueExpression);
            }

            return tagBuilder;
        }

        // ---- Custom ----

        // ko.Html.DropDownListCustom(customBinding, optionsRaw, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, object> DropDownListCustom(
            string customBinding,
            string optionsRaw,
            object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutSelectTagBuilder<TModel, object>(this.Context, optionsRaw, this.InstanceNames, this.Aliases, customBinding);
            tagBuilder.ApplyAttributes(htmlAttributes);
            return tagBuilder;
        }

        // ko.Html.DropDownListCustom(customBinding, valueExpression, optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownListCustom<TItem>(
            string customBinding,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<object, TModel, TItem, object, object>(null, null, optionsExpression, null, null, htmlAttributes, customBinding);
        }

        // ko.Html.DropDownListCustom(customBinding, valueExpression, optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownListCustom<TProperty, TItem>(
            string customBinding,
            Expression<Func<TModel, TProperty>> valueExpression,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<TProperty, TModel, TItem, object, object>(valueExpression, null, optionsExpression, null, null, htmlAttributes, customBinding);
        }

        // ko.Html.DropDownListCustom(customBinding, valueExpression, optionsContext, optionsExpression, [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownListCustom<TProperty, TOptionsModel, TItem>(
            string customBinding,
            Expression<Func<TModel, TProperty>> valueExpression,
            KnockoutContext<TOptionsModel> optionsContext,
            Expression<Func<TOptionsModel, IList<TItem>>> optionsExpression,
            object htmlAttributes = null)
        {
            return this.BuildSelect<TProperty, TOptionsModel, TItem, object, object>(valueExpression, optionsContext, optionsExpression, null, null, htmlAttributes, customBinding);
        }

        // ko.Html.DropDownListCustom(customBinding, valueExpression, optionsExpression, [optionsTextExpression], [optionsValueExpression], [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownListCustom<TProperty, TItem, TTextProperty, TValueProperty>(
            string customBinding,
            Expression<Func<TModel, TProperty>> valueExpression,
            Expression<Func<TModel, IList<TItem>>> optionsExpression,
            Expression<Func<TItem, TTextProperty>> optionsTextExpression = null,
            Expression<Func<TItem, TValueProperty>> optionsValueExpression = null,
            object htmlAttributes = null)
        {
            return this.BuildSelect(valueExpression, this.Context, optionsExpression, optionsTextExpression, optionsValueExpression, htmlAttributes, customBinding);
        }

        // ko.Html.DropDownListCustom(customBinding, valueExpression, optionsContext, optionsExpression, [optionsTextExpression], [optionsValueExpression], [htmlAttributes])

        public KnockoutSelectTagBuilder<TModel, TItem> DropDownListCustom<TProperty, TOptionsModel, TItem, TTextProperty, TValueProperty>(
            string customBinding,
            Expression<Func<TModel, TProperty>> valueExpression,
            KnockoutContext<TOptionsModel> optionsContext,
            Expression<Func<TOptionsModel, IList<TItem>>> optionsExpression,
            Expression<Func<TItem, TTextProperty>> optionsTextExpression = null,
            Expression<Func<TItem, TValueProperty>> optionsValueExpression = null,
            object htmlAttributes = null)
        {
            return this.BuildSelect(valueExpression, optionsContext, optionsExpression, optionsTextExpression, optionsValueExpression, htmlAttributes, customBinding);
        }

        #endregion

        public KnockoutSelectTagBuilder<TModel, TItem> ListBox<TProperty, TItem, TTextProperty, TValueProperty>(Expression<Func<TModel, TProperty>> expression, Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, TTextProperty>> optionsText = null, Expression<Func<TItem, TValueProperty>> optionsValue = null)
        {
            var tagBuilder = this.DropDownList(expression, options, optionsText, optionsValue, htmlAttributes);
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
              actionName, controllerName, routeValues, htmlAttributes, modelData: modelOut, modelDataReturn: modelIn, settings: settings);
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
              actionName, controllerName, routeValues, htmlAttributes, withBinding: bindingName, modelData: modelOut, modelDataReturn: modelIn, settings: settings);
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