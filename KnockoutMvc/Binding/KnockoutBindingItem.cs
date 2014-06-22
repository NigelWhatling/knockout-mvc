namespace KnockoutMvc
{
    using System;
    using System.Text;
    using System.Linq.Expressions;

    public class KnockoutBindingItem
    {
        public IKnockoutContext Context { get; set; }

        public string Name { get; set; }

        protected string SafeName
        {
            get
            {
                string name = this.Name;
                if (name == null)
                {
                    return String.Empty;
                }

                bool isSafe = true;
                for (int i = 0; i < name.Length; i++)
                {
                    char c = name[i];
                    if (c >= '0' && c <= '9')
                    {
                        if (i == 0)
                        {
                            isSafe = false;
                            break;
                        }
                    }
                    else if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '$' || c == '_'))
                    {
                        isSafe = false;
                        break;
                    }
                }

                if (isSafe)
                {
                    return name;
                }
                else
                {
                    return "'" + this.Name + "'";
                }
            }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public Expression Expression { get; set; }

        public KnockoutBindingItem()
        {
        }

        public KnockoutBindingItem(string name, Expression expression)
        {
            this.Name = name;
            this.Expression = expression;
        }

        public KnockoutBindingItem(string name, Expression expression, IKnockoutContext context)
            : this(name, expression)
        {
            this.Context = context;
        }

        public virtual string GetKnockoutExpression(KnockoutExpressionData data)
        {
            string value = KnockoutExpressionConverter.Convert(this.Expression, data);
            if (string.IsNullOrWhiteSpace(value))
                value = "$data";

            var sb = new StringBuilder();

            sb.Append(this.SafeName);
            sb.Append(" : ");
            sb.Append(value);

            return sb.ToString();
        }
    }

    /*
    public abstract class KnockoutBindingItem
    {
        public IKnockoutContext Context { get; set; }

        public string Name { get; set; }

        protected string SafeName
        {
            get
            {
                string name = this.Name;
                if (name == null)
                {
                    return String.Empty;
                }

                bool isSafe = true;
                for (int i = 0; i < name.Length; i++)
                {
                    char c = name[i];
                    if (c >= '0' && c <= '9')
                    {
                        if (i == 0)
                        {
                            isSafe = false;
                            break;
                        }
                    }
                    else if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '$' || c == '_'))
                    {
                        isSafe = false;
                        break;
                    }
                }

                if (isSafe)
                {
                    return name;
                }
                else
                {
                    return "'" + this.Name + "'";
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

        public KnockoutBindingItem()
        {
        }

        public KnockoutBindingItem(string name, Expression<Func<TModel, TResult>> expression)
        {
            this.Name = name;
            this.Expression = expression;
        }

        public KnockoutBindingItem(string name, Expression<Func<TModel, TResult>> expression, IKnockoutContext context)
            : this(name, expression)
        {
            this.Context = context;
        }

        public override string GetKnockoutExpression(KnockoutExpressionData data)
        {
            string value = KnockoutExpressionConverter.Convert(this.Expression, data);
            if (string.IsNullOrWhiteSpace(value))
                value = "$data";

            var sb = new StringBuilder();

            sb.Append(this.SafeName);
            sb.Append(" : ");
            sb.Append(value);

            return sb.ToString();
        }
    }
    */
}
