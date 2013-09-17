namespace PerpetuumSoft.Knockout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class KnockoutExecuteSettings
    {
        public string RequestType { get; set; }
        public bool SendAntiForgeryToken { get; set; }

        public string Complete { get; set; }
        public string Success { get; set; }
        public string Error { get; set; }
        public string BeforeSend { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{");
            bool isFirst = true;
            foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.Name == "SendAntiForgeryToken")
                {
                    continue;
                }

                if (!isFirst)
                {
                    sb.Append(",");
                }

                sb.Append(" ");
                sb.Append(prop.Name.Substring(0, 1).ToLower());
                sb.Append(prop.Name.Substring(1));
                sb.Append(" : ");
                sb.Append(prop.GetValue(this, null));
            }

            sb.Append("}");
            return sb.ToString();
        }
    }
}
