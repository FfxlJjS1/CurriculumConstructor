using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;

namespace CurriculumConstructor.UserClassJsomConverters
{
    public class ListStringConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Логика десериализации из JSON в объект List<string>
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of object.");

            List<string> strings = new List<string>();
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                strings.Add(reader.GetString());
            }

            return strings;
        }

        public override void Write(Utf8JsonWriter writer, List<string> values, JsonSerializerOptions options)
        {
            // Логика сериализации объекта List<string> в JSON
            writer.WriteStartArray();
            
            foreach(string value in values)
            {
                writer.WriteStringValue(value);
            }

            writer.WriteEndArray();
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(string.Join((char)0x00, value));
        }

        public override List<string> ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<string> propertyParts = reader.GetString().Split((char)0x00).ToList();

            return propertyParts;
        }
    }

}
