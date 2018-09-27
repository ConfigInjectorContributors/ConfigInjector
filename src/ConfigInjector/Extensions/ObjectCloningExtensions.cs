using System.IO;
using System.Xml.Serialization;

namespace ConfigInjector.Extensions
{
    internal static class ObjectCloningExtensions
    {
        public static T Clone<T>(this T o)
        {
            // TODO clearly this could afford to be faster. Pull requests gradefully accepted. Eventually ;)  -andrewh
            using (var ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(o.GetType());
                serializer.Serialize(ms, o);
                ms.Position = 0;
                var clone = serializer.Deserialize(ms);
                return (T) clone;
            }
        }
    }
}