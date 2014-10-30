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
            : base(context, "select", instanceNames, aliases)
        {
        }
    }

    public class KnockoutSelectBinding<TModel, TItem> : KnockoutSelectBinding<TModel>
    {
        public KnockoutSelectBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }

        private KnockoutSelectTagBuilder<TModel, TNewItem> NewTagBuilder<TNewItem>()
        {
            KnockoutSelectTagBuilder<TModel, TNewItem> binding = new KnockoutSelectTagBuilder<TModel, TNewItem>(this.Context, this.Context.CreateData().InstanceNames, this.Context.CreateData().Aliases);
            binding.tagBuilder.MergeAttributes(this.tagBuilder.Attributes);
            binding.Items.AddRange(this.Items);
            return binding;
        }

        private KnockoutSelectTagBuilder<TNewModel, TNewItem> NewTagBuilder<TNewModel, TNewItem>(KnockoutContext<TNewModel> context)
        {
            KnockoutSelectTagBuilder<TNewModel, TNewItem> binding = new KnockoutSelectTagBuilder<TNewModel, TNewItem>(context, context.CreateData().InstanceNames, context.CreateData().Aliases);
            binding.tagBuilder.MergeAttributes(this.tagBuilder.Attributes);
            binding.Items.AddRange(this.Items);
            return binding;
        }

        public KnockoutSelectTagBuilder<TModel, TNewItem> Options<TNewItem>(Expression<Func<TModel, ICollection<TNewItem>>> expression, string customBinding = null)
        {
            KnockoutSelectTagBuilder<TModel, TNewItem> binding = this.NewTagBuilder<TNewItem>();
            binding.Items.Add(new KnockoutBindingItem(customBinding ?? "options", expression));
            return binding;
        }

        public KnockoutSelectTagBuilder<TParent, TNewItem> Options<TParent, TNewItem>(KnockoutContext<TParent> context, Expression<Func<TParent, ICollection<TNewItem>>> expression, string customBinding = null)
        {
            KnockoutSelectTagBuilder<TParent, TNewItem> binding = this.NewTagBuilder<TParent, TNewItem>(context);
            binding.Items.Add(new KnockoutBindingItem(customBinding ?? "options", expression));
            return binding;
        }

        public KnockoutSelectTagBuilder<TModel, object> Options(string text, bool isWord = false, string customBinding = null)
        {
            KnockoutSelectTagBuilder<TModel, object> binding = this.NewTagBuilder<object>();
            binding.Items.AddRange(this.Items);
            binding.Items.Add(new KnockoutBindingStringItem(customBinding ?? "options", text, isWord));
            return binding;
        }

        public KnockoutSelectBinding<TModel> OptionsCaption<TProperty>(Expression<Func<TModel, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("optionsCaption", binding));
            return this;
        }

        public KnockoutSelectBinding<TModel> OptionsCaption<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, TProperty>> binding)
        {
            Items.Add(new KnockoutBindingItem("optionsCaption", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel> OptionsCaption(string binding, bool isWord = false)
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
            Items.Add(new KnockoutBindingItem("optionsText", binding, context));
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
            Items.Add(new KnockoutBindingItem("optionsValue", binding, context));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> OptionsValue(string text, bool isWord = false)
        {
            Items.Add(new KnockoutBindingStringItem("optionsValue", text, isWord));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> SelectedOptions(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem("selectedOptions", binding));
            return this;
        }

        public KnockoutSelectBinding<TModel, TItem> SelectedOptions<TProperty, TParent>(KnockoutContext<TParent> context, Expression<Func<TParent, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem("selectedOptions", binding, context));
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
