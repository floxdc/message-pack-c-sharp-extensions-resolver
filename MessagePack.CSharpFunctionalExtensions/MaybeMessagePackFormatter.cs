using System;
using CSharpFunctionalExtensions;
using MessagePack.Formatters;

namespace MessagePack.CSharpFunctionalExtensions
{
    public class MaybeMessagePackFormatter<T> : IMessagePackFormatter<Maybe<T>>
    {
        public Maybe<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
                return default;
            
            var count = reader.ReadArrayHeader();
            if (count != HeaderCount)
                throw new InvalidOperationException();

            if (!reader.ReadBoolean())
                return Maybe<T>.None;

            var formatter = options.Resolver.GetFormatterWithVerify<T>();
            var value = formatter.Deserialize(ref reader, options);

            return Maybe<T>.From(value);
        }


        public void Serialize(ref MessagePackWriter writer, Maybe<T> value, MessagePackSerializerOptions options)
        {
            if (value.Equals(default))
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(HeaderCount);
            writer.Write(value.HasValue);

            if (value.HasValue)
            {
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value.Value, options);
                
                return;
            }

            writer.WriteNil();
        }


        private const int HeaderCount = 2;
    }
}
