using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace AntiAntiSwearingBot
{
    public abstract class ConfigBase
    {
        public static T Load<T>(params string[] paths) where T : ConfigBase, new()
        {
            var result = new T();
            var configJson = new JObject();
            var mergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            };

            foreach (var path in paths)
            {
                try { configJson.Merge(JObject.Parse(File.ReadAllText(path)), mergeSettings);}
                catch { }
            }

            using (var sr = configJson.CreateReader())
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver()
                };
                JsonSerializer.CreateDefault(settings).Populate(sr, result);
            }

            return result;
        }
    }

    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }

    public class PrivateSetterCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }

    internal static class MemberInfoExtensions
    {
        internal static bool IsPropertyWithSetter(this MemberInfo member)
        {
            var property = member as PropertyInfo;

            return property?.GetSetMethod(true) != null;
        }
    }
}
