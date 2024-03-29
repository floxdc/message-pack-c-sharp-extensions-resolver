﻿using System;
using CSharpFunctionalExtensions;
using MessagePack.Formatters;

namespace MessagePack.CSharpFunctionalExtensions
{
    public class ResultOfTMessagePackFormatter<T> : IMessagePackFormatter<Result<T>>
    {

        public Result<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
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

                return Result.Success(value);
            }

            var error = reader.ReadString();
            return Result.Failure<T>(error);
        }


        public void Serialize(ref MessagePackWriter writer, Result<T> value, MessagePackSerializerOptions options)
        {
            if (value.Equals(default(Result<T>)))
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(HeaderCount);
            writer.Write(value.IsFailure);

            if (value.IsFailure)
            {
                writer.Write(value.Error);
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
