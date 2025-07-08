using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.HelperServices;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private const string Format = "HH:mm:ss";
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString()!;
        // если всё-таки прилетел ISO‑стринг с Z или миллисекундами — обрежем лишнее
        if (s.EndsWith("Z")) s = s[..^1];
        var dot = s.IndexOf('.');
        if (dot >= 0) s = s[..dot];
        return TimeOnly.ParseExact(s, Format, CultureInfo.InvariantCulture);
    }
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format));
}