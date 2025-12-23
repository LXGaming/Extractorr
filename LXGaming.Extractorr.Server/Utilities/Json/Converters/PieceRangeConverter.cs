using System.Text.Json;
using System.Text.Json.Serialization;
using LXGaming.Extractorr.Server.Services.QBittorrent.Models;

namespace LXGaming.Extractorr.Server.Utilities.Json.Converters;

public class PieceRangeConverter : JsonConverter<PieceRange> {

    public override PieceRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.StartArray) {
            var array = JsonSerializer.Deserialize<int[]>(ref reader, options);
            if (array is not { Length: 2 }) {
                throw new JsonException("Invalid array length.");
            }

            return new PieceRange {
                Start = array[0],
                End = array[1]
            };
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing {typeToConvert.Name}.");
    }

    public override void Write(Utf8JsonWriter writer, PieceRange value, JsonSerializerOptions options) {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Start);
        writer.WriteNumberValue(value.End);
        writer.WriteEndArray();
    }
}