using System;
using System.Collections.Generic;
using System.Reflection;
using CSharpFunctionalExtensions;
using MessagePack.Formatters;

namespace MessagePack.CSharpFunctionalExtensions
{
    public class CSharpFunctionalExtensionsFormatResolver : IFormatterResolver
    {
        private CSharpFunctionalExtensionsFormatResolver()
        { }


        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;


        public static readonly IFormatterResolver Instance = new CSharpFunctionalExtensionsFormatResolver();
    }


    internal static class FormatterCache<T>
    {
        static FormatterCache()
        {
            Formatter = (IMessagePackFormatter<T>) CSharpFunctionalExtensionsGetFormatterHelper.GetFormatter(typeof(T));
        }


        public static readonly IMessagePackFormatter<T> Formatter;
    }


    internal class CSharpFunctionalExtensionsGetFormatterHelper
    {
        internal static object GetFormatter(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType)
                type = typeInfo.GetGenericTypeDefinition()
                    .GetTypeInfo();

            FormatterMap.TryGetValue(type, out var formatter);
            return typeInfo.IsGenericType 
                ? CreateInstance(formatter as Type, typeInfo.GenericTypeArguments) 
                : formatter;
        }


        private static object CreateInstance(Type genericType, Type[] genericTypeArguments, params object[] arguments)
        {
            return Activator.CreateInstance(genericType.MakeGenericType(genericTypeArguments), arguments);
        }


        private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>
        {
            {typeof(Result), ResultMessagePackFormatter.Instance},
            {typeof(Result<>), typeof(ResultOfTMessagePackFormatter<>)},
            {typeof(Result<,>), typeof(ResultOfTTeEMessagePackFormatter<,>)},
            {typeof(Maybe<>), typeof(MaybeMessagePackFormatter<>)}
        };
    }
}