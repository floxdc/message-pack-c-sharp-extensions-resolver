using System;
using CSharpFunctionalExtensions;
using MessagePack.Formatters;

namespace MessagePack.CSharpFunctionalExtensions
{
    public class ResultOfTTeEMessagePackFormatter<T,TE> : IMessagePackFormatter<Result<T,TE>>
    {
        public Result<T,TE> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
                return default;

            var count = reader.ReadArrayHeader();
            if (count != HeaderCount)
                throw new InvalidOperationException();

            if (!reader.ReadBoolean())
            {
                reader.ReadNil();
                
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var value = formatter.Deserialize(ref reader, options);

                return Result.Ok<T,TE>(value);
            }
            else
            {
                var formatter = options.Resolver.GetFormatterWithVerify<TE>();
                var error = formatter.Deserialize(ref reader, options);
                return Result.Failure<T,TE>(error);
            }
        }


        public void Serialize(ref MessagePackWriter writer, Result<T,TE> value, MessagePackSerializerOptions options)
        {
            if (value.Equals(default(Result<T,TE>)))
            {
                writer.WriteNil();
                return;
            }


            writer.WriteArrayHeader(HeaderCount);
            writer.Write(value.IsFailure);

            if (value.IsFailure)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<TE>();
                formatter.Serialize(ref writer, value.Error, options);

                writer.WriteNil();
            }
            else
            {
                writer.WriteNil();

                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value.Value, options);
            }
        }


        private const int HeaderCount = 3;
    }
}
