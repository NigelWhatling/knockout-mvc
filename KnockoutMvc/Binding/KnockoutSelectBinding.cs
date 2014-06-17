namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq.Expressions;
    using System.Web;

    public class KnockoutSelectBinding<TModel> : KnockoutBinding<TModel>
    {
        public KnockoutSelectBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }
    }

    public class KnockoutSelectBinding<TModel, TItem> : KnockoutSelectBinding<TModel>
    {
        public KnockoutSelectBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsCaption<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, TProperty>("optionsCaption", binding));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsCaption<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("optionsCaption", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsCaption(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsCaption", binding));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsText<TProperty>(Expression<Func<TItem, TProperty>> binding)
        {
            var data = new KnockoutExpressionData { InstanceNames = new[] { "item" } };
            string value = KnockoutExpressionConverter.Convert(binding, data);

            if (binding is LambdaExpression && binding.Body is MemberExpression && value == String.Format("item.{0}", ((MemberExpression)binding.Body).Member.Name))
            {
                this.Items.Add(new KnockoutBindingStringItem("optionsText", ((MemberExpression)binding.Body).Member.Name));
            }
            else
            {
                this.Items.Add(new KnockoutBindingStringItem("optionsText", "function(item) { return " + value + "; }", false));
            }

            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsText<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("optionsText", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsText(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsText", text, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsValue<TProperty>(Expression<Func<TItem, TProperty>> binding)
        {
            var data = new KnockoutExpressionData { InstanceNames = new[] { "item" } };
            string value = KnockoutExpressionConverter.Convert(binding, data);

            if (binding is LambdaExpression && binding.Body is MemberExpression && value == String.Format("item.{0}", ((MemberExpression)binding.Body).Member.Name))
            {
                this.Items.Add(new KnockoutBindingStringItem("optionsValue", ((MemberExpression)binding.Body).Member.Name));
            }
            else
            {
                this.Items.Add(new KnockoutBindingStringItem("optionsValue", "function(item) { return " + value + "; }", false));
            }

            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsValue<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, TProperty>("optionsValue", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsValue", text, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> SelectedOptions(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable>("selectedOptions", binding));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> SelectedOptions<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TParent, IEnumerable>("selectedOptions", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> SelectedOptions(string binding, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("selectedOptions", binding, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> ValueAllowUnset(bool isAllowed = true)
        {
            Items.Add(new KnockoutBindingStringItem("valueAllowUnset", isAllowed.ToString().ToLower(), false));
            return this;
        }
    }
}
