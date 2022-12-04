using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using todo_domain_entities;

namespace todo_aspnetmvc_ui.Infrastructure
{
    public class ToDoStatusJsonConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ToDoStatus status)
            {
                writer.WriteValue(status.GetAttribute<DisplayAttribute>().Name);
                return;
            }

            base.WriteJson(writer, value, serializer);
        }
    }
}
