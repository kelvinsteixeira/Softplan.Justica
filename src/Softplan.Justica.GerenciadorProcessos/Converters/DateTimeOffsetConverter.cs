using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Softplan.Justica.GerenciadorProcessos.Converters
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private const string DateFormmat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return DateTimeOffset.Parse(str);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormmat, CultureInfo.InvariantCulture));
        }
    }
}