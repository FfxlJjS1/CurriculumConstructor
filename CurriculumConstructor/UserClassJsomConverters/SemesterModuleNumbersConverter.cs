using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;

namespace CurriculumConstructor.UserClassJsomConverters
{
    public class SemesterModuleNumbersConverter : JsonConverter<SemesterModuleNumbers>
    {
        public override SemesterModuleNumbers Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Логика десериализации из JSON в объект SemesterModuleNumbers
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object.");

            int semesterNumber = 0;
            int semesterModuleNumber = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "SemesterNumber":
                            semesterNumber = reader.GetInt32();
                            break;
                        case "SemesterModuleNumber":
                            semesterModuleNumber = reader.GetInt32();
                            break;
                        default:
                            throw new JsonException($"Unexpected property: {propertyName}");
                    }
                }
            }

            return new SemesterModuleNumbers(semesterNumber, semesterModuleNumber);
        }

        public override void Write(Utf8JsonWriter writer, SemesterModuleNumbers value, JsonSerializerOptions options)
        {
            // Логика сериализации объекта SemesterModuleNumbers в JSON
            writer.WriteStartObject();
            writer.WriteNumber("SemesterNumber", value.SemesterNumber);
            writer.WriteNumber("SemesterModuleNumber", value.SemesterModuleNumber);
            writer.WriteEndObject();
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, SemesterModuleNumbers value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.SemesterNumber.ToString() + ":" + value.SemesterModuleNumber.ToString());
        }

        public override SemesterModuleNumbers ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string[] propertyParts = reader.GetString().Split(":");

            return new SemesterModuleNumbers(Convert.ToInt32(propertyParts[0]), Convert.ToInt32(propertyParts[1]));
        }
    }

}
