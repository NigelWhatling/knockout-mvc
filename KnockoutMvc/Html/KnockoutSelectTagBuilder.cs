namespace KnockoutMvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web.Mvc;

    public class KnockoutSelectTagBuilder<TModel, TItem> : KnockoutSelectBinding<TModel, TItem>, IKnockoutTagBuilder<TModel>, IKnockoutTagBuilder
    {
        public KnockoutSelectTagBuilder(KnockoutContext<TModel> context, string[] instanceNames, Dictionary<string, string> aliases)
            : base(context, instanceNames, aliases)
        {
            TagRenderMode = TagRenderMode.Normal;
        }

        public KnockoutSelectTagBuilder(KnockoutContext<TModel> context, IKnockoutContext optionsContext, Expression options, string[] instanceNames, Dictionary<string, string> aliases, string customBinding = null)
            : base(context, instanceNames, aliases)
        {
            TagRenderMode = TagRenderMode.Normal;
            this.Items.Add(new KnockoutBindingItem(customBinding ?? "options", options, optionsContext ?? context));
        }

        public KnockoutSelectTagBuilder(KnockoutContext<TModel> context, string text, string[] instanceNames, Dictionary<string, string> aliases, string customBinding = null)
            : base(context, instanceNames, aliases)
        {
            TagRenderMode = TagRenderMode.Normal;
            this.Items.Add(new KnockoutBindingStringItem(customBinding ?? "options", text, false));
        }

        public void ApplyAttributes(object htmlAttributes)
        {
            this.ApplyAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public void ApplyAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes != null)
            {
                foreach (var htmlAttribute in htmlAttributes)
                {
                    tagBuilder.Attributes[htmlAttribute.Key] = Convert.ToString(htmlAttribute.Value);
                }
            }
        }

        public void ApplyAttribute(string name, object value)
        {
            tagBuilder.Attributes[name] = Convert.ToString(value);
        }

        public void RemoveAttribute(string name)
        {
            if (tagBuilder.Attributes.ContainsKey(name))
            {
                tagBuilder.Attributes.Remove(name);
            }
        }

        public TagRenderMode TagRenderMode { get; set; }

        public string ToHtmlStringNoBinding()
        {
            return tagBuilder.ToString(TagRenderMode);
        }

        public override string ToHtmlString()
        {
            string bindContent = this.BindingAttributeContent();
            if (!String.IsNullOrWhiteSpace(bindContent))
            {
                tagBuilder.Attributes["data-bind"] = BindingAttributeContent();
            }

            return tagBuilder.ToString(TagRenderMode);
        }
    }
}