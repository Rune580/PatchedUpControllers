using System.Text;
using Newtonsoft.Json;

namespace PatchedUpControllers.Utils;

internal static class JsonUtils
{
    public static T DeserializeAs<T>(this string json)
    {
        JsonSerializer serializer = JsonSerializer.CreateDefault();
        
        using var stringReader = new StringReader(json);
        using var reader = new JsonTextReader(stringReader);
        
        T data = serializer.Deserialize<T>(reader);

        return data;
    }

    public static string Serialize<T>(this T data)
    {
        JsonSerializer serializer = JsonSerializer.CreateDefault();
        
        StringBuilder builder = new StringBuilder();
        using var stringWriter = new StringWriter(builder);
        using var writer = new JsonTextWriter(stringWriter);

        serializer.Serialize(writer, data);

        return builder.ToString();
    }
}