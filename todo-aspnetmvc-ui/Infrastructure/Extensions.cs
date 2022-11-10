using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace todo_aspnetmvc_ui.Infrastructure
{
    public static class Extensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static T GetValueFromName<T>(this string name)
            where T : Enum
        {
            var type = typeof(T);

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) 
                    is DisplayAttribute attribute && attribute.Name == name)
                {
                    return (T)field.GetValue(null);
                }

                if (field.Name == name)
                {
                    return (T)field.GetValue(null);
                }
            }

            return default;
        }
    }
}
