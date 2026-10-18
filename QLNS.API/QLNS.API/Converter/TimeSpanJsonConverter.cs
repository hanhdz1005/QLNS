using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QLNS.API.Converter
{
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (TimeSpan.TryParseExact(value, ["hh\\:mm\\:ss", "hh\\:mm"], CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(@"hh\:mm\:ss"));
            else
                writer.WriteNullValue();
        }
    }
}
