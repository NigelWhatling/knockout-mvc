namespace KnockoutMvc
{
    using System.Collections.Generic;

    public abstract class KnockoutSubContext<TModel>
    {
        internal KnockoutContext<TModel> Context { get; set; }
        internal string[] InstanceNames { get; set; }
        internal Dictionary<string, string> Aliases { get; set; }

        protected KnockoutSubContext(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
        {
            this.Context = context;
            this.InstanceNames = instanceNames;
            this.Aliases = aliases;
        }

        protected KnockoutExpressionData CreateData()
        {
            var data = new KnockoutExpressionData();
            if (this.InstanceNames != null)
                data.InstanceNames = this.InstanceNames;
            if (this.Aliases != null)
                data.Aliases = this.Aliases;
            return data.Clone();
        }
    }
}
