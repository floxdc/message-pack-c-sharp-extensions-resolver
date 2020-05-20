using CSharpFunctionalExtensions;
using MessagePack.CSharpFunctionalExtensions;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.CSharpFunctionalExtensionsTests
{
    public class ResultOfTMessagePackFormatterTests
    {
        public ResultOfTMessagePackFormatterTests()
        {
            var resolver = CompositeResolver.Create(StandardResolver.Instance, CSharpFunctionalExtensionsFormatResolver.Instance);
            _options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);
        }


        [Fact]
        public void ShouldHandleDefault()
        {
            var result = default(Result<DefaultClass>);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass>>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
            Assert.Null(deserialized.Value);
        }


        [Fact]
        public void ShouldHandleFail()
        {
            const string error = "test error Message";
            var result = Result.Failure<DefaultClass>(error);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass>>(serialized, _options);

            Assert.False(deserialized.IsSuccess);
            Assert.True(deserialized.IsFailure);
            Assert.Equal(error, deserialized.Error);
        }
        
        
        [Fact]
        public void ShouldHandleOk()
        {
            var obj = new DefaultClass
            {
                IntValue = 42,
                StringValue = "test str"
            };

            var result = Result.Ok(obj);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass>>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
        }
    
        
        private readonly MessagePackSerializerOptions _options;
    }
}
