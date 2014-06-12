namespace KnockoutMvc
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    public class KnockoutFormContext<TModel> : KnockoutRegionContext<TModel>
    {
        private readonly KnockoutContext<TModel> context;
        private readonly string[] instanceNames;
        private readonly Dictionary<string, string> aliases;

        private readonly string actionName;
        private readonly string controllerName;
        private readonly object routeValues;
        private readonly object htmlAttributes;
        private readonly string withBinding;
        private readonly string bindingOut;
        private readonly string bindingIn;
        private readonly KnockoutExecuteSettings settings;

        public KnockoutFormContext(
          KnockoutContext<TModel> context, string[] instanceNames, Dictionary<string, string> aliases,
          string actionName, string controllerName, object routeValues, object htmlAttributes, string withBinding = null, string bindingOut = null, string bindingIn = null, KnockoutExecuteSettings settings = null)
            : base(context)
        {
            this.context = context;
            this.instanceNames = instanceNames;
            this.aliases = aliases;
            this.actionName = actionName;
            this.controllerName = controllerName;
            this.routeValues = routeValues;
            this.htmlAttributes = htmlAttributes;
            this.withBinding = withBinding;
            this.bindingOut = bindingOut;
            this.bindingIn = bindingIn;
            this.settings = settings;
            InStack = false;
        }

        public override void WriteStart(TextWriter writer)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(context, "form", instanceNames, aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (this.withBinding != null)
            {
                tagBuilder.Custom("with", this.withBinding);
            }

            tagBuilder.Submit(actionName, controllerName, routeValues, bindingOut: this.bindingOut, bindingIn: this.bindingIn, settings: this.settings);
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            writer.WriteLine(tagBuilder.ToHtmlString());
        }

        protected override void WriteEnd(TextWriter writer)
        {
            var tagBuilder = new TagBuilder("form");
            writer.WriteLine(new MvcHtmlString(tagBuilder.ToString(TagRenderMode.EndTag)));
        }
    }
}