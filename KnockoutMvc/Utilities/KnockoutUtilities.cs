namespace KnockoutMvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class KnockoutUtilities
    {
        private static readonly List<AssemblyName> systemNames = new List<AssemblyName>
        {
            new AssemblyName ("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
            new AssemblyName ("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
        };

        public static void ConvertData(object data)
        {
            if (data == null)
                return;
            var type = data.GetType();
            if (type.Namespace == null || typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                return;
            if (type.IsClass && type.Namespace.Equals("System.Data.Entity.DynamicProxies"))
                type = type.BaseType;
            bool isDataContract = type.GetCustomAttributes<System.Runtime.Serialization.DataContractAttribute>().Count() > 0;
            foreach (var property in type.GetProperties())
            {
                if (isDataContract && property.GetCustomAttributes<System.Runtime.Serialization.DataMemberAttribute>().Count() == 0)
                {
                    continue;
                }
                
                bool isDataMember = true;
                foreach (object attribute in property.GetCustomAttributes(false))
                {
                    if (attribute is System.Runtime.Serialization.IgnoreDataMemberAttribute ||
                        attribute is Newtonsoft.Json.JsonIgnoreAttribute)
                    {
                        isDataMember = false;
                        break;
                    }
                }

                if (!isDataMember)
                {
                    continue;
                }

                //if (property.GetCustomAttributes(typeof(Newtonsoft.Json.JsonIgnoreAttribute), false).Length > 0)
                //    continue;
                if (property.GetGetMethod() == null)
                    continue;
                if (property.GetGetMethod().GetParameters().Length > 0)
                    continue;
                var value = property.GetValue(data, null);
                if (value == null)
                {
                    value = GetActualValue(property.PropertyType, null);
                    if (value != null && property.CanWrite)
                        property.SetValue(data, value, null);
                }
                else if (!IsSystemType(property.PropertyType))
                    ConvertData(value);
            }
        }

        public static object GetActualValue(Type type, object value)
        {
            if (value == null)
            {
                //if (typeof(string).IsAssignableFrom(type))
                //{
                //    return "";
                //}
                if (typeof(ICollection).IsAssignableFrom(type))
                {
                    ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    if (constructor != null)
                    {
                        return constructor.Invoke(null);
                    }
                }
            }
            return value;
        }

        private static bool IsSystemType(Type type)
        {
            var objAN = new AssemblyName(type.Assembly.FullName);
            return systemNames.Any(n => n.Name == objAN.Name &&
                                        n.GetPublicKeyToken().SequenceEqual(objAN.GetPublicKeyToken()));
        }
    }
}
