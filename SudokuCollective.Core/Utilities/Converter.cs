using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuCollective.Core.Interfaces.Models;

namespace SudokuCollective.Core.Utilities
{
    public class IDomainEntityListConverter<T> : JsonConverter<T> where T : class
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(ref reader, options);

        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value is IEnumerable<IDomainEntity> entities)
            {
                foreach (var entity in entities)
                {
                    writer.WriteRawValue(entity.ToJson());
                }
            }

            writer.WriteEndArray();
        }
    }
}
