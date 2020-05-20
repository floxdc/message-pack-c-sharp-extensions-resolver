using System;
using CSharpFunctionalExtensions;
using MessagePack.Formatters;

namespace MessagePack.CSharpFunctionalExtensions
{
    public class ResultMessagePackFormatter : IMessagePackFormatter<Result>
    {
        public static readonly ResultMessagePackFormatter Instance = new ResultMessagePackFormatter();


        public Result Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
                return default;

            var count = reader.ReadArrayHeader();
            if (count != HeaderCount)
                throw new InvalidOperationException();

            if (!reader.ReadBoolean())
                return Result.Ok();

            var error = reader.ReadString();
            return Result.Failure(error);

        }


        public void Serialize(ref MessagePackWriter writer, Result value, MessagePackSerializerOptions options)
        {
            if (value.Equals(default(Result)))
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(HeaderCount);
            writer.Write(value.IsFailure);

            if (value.IsFailure)
                writer.Write(value.Error);
            else
                writer.WriteNil();
        }


        private const int HeaderCount = 2;
    }
}
