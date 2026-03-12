//namespace Amigo.SharedKernal.JsonConvertors;

//internal class ExceptionConverter : JsonConverter<Exception>
//{
//    public override Exception? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        throw new NotSupportedException("Deserializing exceptions is not supported.");
//    }

//    public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();

//        writer.WriteString(nameof(value.Message).ToLower(), value.Message);
//        writer.WriteString(nameof(value.Source).ToLower(), value.Source);
//        writer.WriteString("stackTrace", value.StackTrace);

//        if (value.InnerException is not null)
//        {
//            writer.WritePropertyName("innerException");
//            JsonSerializer.Serialize(writer, value.InnerException, options);
//        }

//        writer.WriteEndObject();
//    }
//}