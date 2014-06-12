namespace KnockoutMvc
{
    using System;
    using System.Text;
    using System.Linq.Expressions;

    public abstract class KnockoutBindingItem
    {
        public string Name { get; set; }

        protected string SafeName
        {
            get
            {
                if (this.Name.IndexOf('-') > 0)
                {
                    return "'" + this.Name + "'";
                }
                else
                {
                    return this.Name;
                }
            }
        }

        public abstract string GetKnockoutExpression(KnockoutExpressionData data);

        public virtual bool IsValid()
        {
            return true;
        }
    }

    public class KnockoutBindingItem<TModel, TResult> : KnockoutBindingItem
    {
        public Expression<Func<TModel, TResult>> Expression { get; set; }

        public override string GetKnockoutExpression(KnockoutExpressionData data)
        {
            string value = KnockoutExpressionConverter.Convert(Expression, data);
            if (string.IsNullOrWhiteSpace(value))
                value = "$data";

            var sb = new StringBuilder();

            sb.Append(this.SafeName);
            sb.Append(" : ");
            sb.Append(value);

            return sb.ToString();
        }
    }
}
