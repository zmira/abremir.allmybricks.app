using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace abremir.AllMyBricks.Onboarding.Helpers
{
    internal class CustomDateTimeOffsetConverter(string format) : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), format, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format));
        }
    }
}
