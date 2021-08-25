using CSharpFunctionalExtensions;
using MessagePack.CSharpFunctionalExtensions;
using Xunit;

namespace MessagePack.CSharpFunctionalExtensionsTests
{
    public class ResultMessagePackFormatterTests
    {
        public ResultMessagePackFormatterTests()
        {
            _options = MessagePackSerializerOptions.Standard
                .WithResolver(CSharpFunctionalExtensionsFormatResolver.Instance);
        }


        [Fact]
        public void ShouldHandleDefault()
        {
            var result = default(Result);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
        }
        
        
        [Fact]
        public void ShouldHandleFail()
        {
            var result = Result.Success();

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result>(serialized, _options);

            Assert.True(deserialized.IsSuccess);
            Assert.False(deserialized.IsFailure);
        }


        [Fact]
        public void ShouldHandleOk()
        {
            const string error = "test error Message";
            var result = Result.Failure(error);

            var serialized = MessagePackSerializer.Serialize(result, _options);
            var deserialized = MessagePackSerializer.Deserialize<Result>(serialized, _options);

            Assert.False(deserialized.IsSuccess);
            Assert.True(deserialized.IsFailure);
            Assert.Equal(error, deserialized.Error);
        }
    
        
        private readonly MessagePackSerializerOptions _options;
    }
}
