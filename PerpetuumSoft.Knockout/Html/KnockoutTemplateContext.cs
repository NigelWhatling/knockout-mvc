using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
  public class KnockoutTemplateContext<TModel> : KnockoutRegionContext<TModel>
  {
    private readonly KnockoutContext<TModel> context;
    private readonly string[] instanceNames;
    private readonly Dictionary<string, string> aliases;

    private readonly string templateId;

    public KnockoutTemplateContext(
      ViewContext viewContext,
      KnockoutContext<TModel> context, string[] instanceNames, Dictionary<string, string> aliases, string templateId)
      : base(viewContext)
    {
      this.context = context;
      this.instanceNames = instanceNames;
      this.aliases = aliases;
      this.templateId = templateId;
      InStack = false;
    }

    public override void WriteStart(TextWriter writer)
    {
      var tagBuilder = new KnockoutTagBuilder<TModel>(context, "script", instanceNames, aliases);
      tagBuilder.ApplyAttributes(new { type = "text/html", id = this.templateId });
      tagBuilder.TagRenderMode = TagRenderMode.StartTag;
      writer.WriteLine(tagBuilder.ToHtmlStringNoBinding());
    }

    protected override void WriteEnd(TextWriter writer)
    {
      var tagBuilder = new TagBuilder("script");
      writer.WriteLine(new MvcHtmlString(tagBuilder.ToString(TagRenderMode.EndTag)));
    }
  }
}