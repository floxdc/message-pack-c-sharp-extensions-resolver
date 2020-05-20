using CSharpFunctionalExtensions;
using MessagePack.CSharpFunctionalExtensions;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.CSharpFunctionalExtensionsTests
{
    public class ResultOfTTeMessagePackFormatterTests
    {
        public ResultOfTTeMessagePackFormatterTests()
        {
            var resolver = CompositeResolver.Create(StandardResolver.Instance, CSharpFunctionalExtensionsFormatResolver.Instance);
            _options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);
        }


        [Fact]
        public void ShouldHandleDefault()
        {
            var result = default(Result<DefaultClass, DefaultClass>);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass, DefaultClass>>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
            Assert.Null(deserialized.Value);
        }


        [Fact]
        public void ShouldHandleFail()
        {
            var err = new DefaultClass
            {
                IntValue = -1,
                StringValue = "test error Message"
            };
            var result = Result.Failure<DefaultClass, DefaultClass>(err);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass, DefaultClass>>(serialized, _options);

            Assert.False(deserialized.IsSuccess);
            Assert.True(deserialized.IsFailure);
        }
        
        
        [Fact]
        public void ShouldHandleOk()
        {
            var obj = new DefaultClass
            {
                IntValue = 42,
                StringValue = "test str"
            };

            var result = Result.Ok<DefaultClass, DefaultClass>(obj);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result<DefaultClass, DefaultClass>>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
        }
    
        
        private readonly MessagePackSerializerOptions _options;
    }
}
