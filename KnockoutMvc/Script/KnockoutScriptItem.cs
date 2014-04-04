namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;

    public class KnockoutScriptItem : IHtmlString
    {
        private string _Text;

        public KnockoutScriptItem(string text)
        {
            this._Text = text;
        }

        public string ToHtmlString()
        {
            return this._Text;
        }

        public override string ToString()
        {
            return this.ToHtmlString();
        }
    }
}