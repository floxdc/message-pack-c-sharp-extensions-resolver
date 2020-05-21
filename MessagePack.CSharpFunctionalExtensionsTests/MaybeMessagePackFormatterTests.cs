using CSharpFunctionalExtensions;
using MessagePack.CSharpFunctionalExtensions;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.CSharpFunctionalExtensionsTests
{
    public class MaybeMessagePackFormatterTests
    {
        public MaybeMessagePackFormatterTests()
        {
            var resolver = CompositeResolver.Create(StandardResolver.Instance, CSharpFunctionalExtensionsFormatResolver.Instance);
            _options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);
        }


        [Fact]
        public void ShouldHandleDefault()
        {
            var result = default(Maybe<DefaultClass>);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Maybe<DefaultClass>>(serialized, _options);

            Assert.True(deserialized.HasNoValue);
        }


        [Fact]
        public void ShouldHandleNone()
        {
            var result = Maybe<DefaultClass>.None;

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Maybe<DefaultClass>>(serialized, _options);

            Assert.False(deserialized.HasValue);
        }
        
        
        [Fact]
        public void ShouldHandleHasValue()
        {
            var obj = new DefaultClass
            {
                IntValue = 42,
                StringValue = "test str"
            };

            var result = Maybe<DefaultClass>.From(obj);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Maybe<DefaultClass>>(serialized, _options);

            Assert.True(deserialized.HasValue);
        }
    
        
        private readonly MessagePackSerializerOptions _options;
    }
}
