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
            Context = context;
            InstanceNames = instanceNames;
            Aliases = aliases;
        }

        protected KnockoutExpressionData CreateData()
        {
            var data = new KnockoutExpressionData();
            if (InstanceNames != null)
                data.InstanceNames = InstanceNames;
            if (Aliases != null)
                data.Aliases = Aliases;
            return data.Clone();
        }
    }
}
