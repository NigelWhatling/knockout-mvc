namespace KnockoutMvc
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    public interface IKnockoutTagBuilder
    {
    }

    public interface IKnockoutTagBuilder<TModel> : IKnockoutBinding<TModel>, IKnockoutTagBuilder
    {
        void ApplyAttributes(object htmlAttributes);
        void ApplyAttributes(IDictionary<string, object> htmlAttributes);
        void ApplyAttribute(string name, object value);
        void RemoveAttribute(string name);
    }

    public class KnockoutTagBuilder<TModel> : KnockoutBinding<TModel>, IKnockoutTagBuilder<TModel>
    {
        public KnockoutTagBuilder(KnockoutContext<TModel> context, string tagName, string[] instanceNames, Dictionary<string, string> aliases)
            : base(context, tagName, instanceNames, aliases)
        {
            TagRenderMode = TagRenderMode.Normal;
        }

        public void ApplyAttributes(object htmlAttributes)
        {
            ApplyAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
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

        public string InnerHtml
        {
            get
            {
                return tagBuilder.InnerHtml;
            }
            set
            {
                tagBuilder.InnerHtml = value;
            }
        }

        public KnockoutTagBuilder<TModel> SetInnerHtml(string innerHtml)
        {
            this.InnerHtml = innerHtml;
            return this;
        }

        public KnockoutTagBuilder<TModel> SetInnerText(string innerText)
        {
            tagBuilder.SetInnerText(innerText);
            return this;
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

        private enum ScanMode
        {
            Start,
            TagName,
            AttrSearch,
            AttrName,
            AttrQuote,
            AttrValue,
            InnerHtml
        }

        public static KnockoutTagBuilder<TModel> CreateFromHtml(KnockoutContext<TModel> context, string element, string[] instanceNames, Dictionary<string, string> aliases)
        {
            KnockoutTagBuilder<TModel> tagBuilder = null;

            int len = element.Length;
            int i = 0;
            StringBuilder sb = new StringBuilder();
            char quote = (char)0;
            string tagName = null;
            string attrName = null;
            ScanMode mode = ScanMode.Start;

            while (i < len)
            {
                if (mode == ScanMode.InnerHtml)
                {
                    string innerHtml = element.Substring(i);
                    int i2 = innerHtml.IndexOf("</" + tagName + ">");
                    if (i2 < 0)
                    {
                        innerHtml = null; 
                        break;
                    }

                    innerHtml = innerHtml.Substring(0, i2);
                    tagBuilder.SetInnerHtml(innerHtml);
                }

                char c = element[i++];
                bool fail = false;

                switch (c)
                {
                    case '<':
                        if (mode == ScanMode.Start)
                        {
                            mode = ScanMode.TagName;
                            sb.Clear();
                        }
                        else
                        {
                            fail = true;
                        }

                        break;

                    case ' ':
                        if (mode == ScanMode.TagName)
                        {
                            mode = ScanMode.AttrSearch;
                            tagName = sb.ToString();
                            tagBuilder = new KnockoutTagBuilder<TModel>(context, tagName, instanceNames, aliases);
                            sb.Clear();
                        }
                        else if (mode == ScanMode.AttrValue)
                        {
                            sb.Append(c);
                        }

                        break;

                    case '=':
                        if (mode == ScanMode.AttrName)
                        {
                            mode = ScanMode.AttrQuote;
                            attrName = sb.ToString();
                            sb.Clear();
                        }
                        else if (mode == ScanMode.AttrValue)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            fail = true;
                        }

                        break;

                    case '\'':
                    case '\"':
                        if (mode == ScanMode.AttrQuote)
                        {
                            mode = ScanMode.AttrValue;
                            quote = c;
                            sb.Clear();
                        }
                        else if (mode == ScanMode.AttrValue)
                        {
                            if (c == quote)
                            {
                                mode = ScanMode.AttrSearch;
                                tagBuilder.ApplyAttribute(attrName, sb.ToString());
                                attrName = null;
                                sb.Clear();
                            }
                            else
                            {
                                sb.Append(c);
                            }
                        }
                        else
                        {
                            fail = true;
                        }

                        break;

                    case '/':
                        if (mode == ScanMode.AttrValue)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            fail = true;
                        }

                        break;

                    case '>':
                        if (mode == ScanMode.AttrValue)
                        {
                            sb.Append(c);
                        }
                        else if (mode == ScanMode.AttrSearch)
                        {
                            mode = ScanMode.InnerHtml;
                        }
                        else
                        {
                            fail = true;
                        }

                        break;

                    default:
                        if (mode == ScanMode.AttrSearch)
                        {
                            mode = ScanMode.AttrName;
                            sb.Clear();
                            sb.Append(c);
                        }
                        else if (mode == ScanMode.TagName || mode == ScanMode.AttrName || mode == ScanMode.AttrValue)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            fail = true;
                        }

                        break;
                }

                if (fail)
                {
                    break;
                }
            }

            return tagBuilder;
        }

    }
}